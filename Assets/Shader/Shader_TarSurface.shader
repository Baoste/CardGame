Shader "Custom/URP/TarSurface"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (0.02, 0.02, 0.025, 1)
        _DeepColor ("Deep Color", Color) = (0.0, 0.0, 0.0, 1)
        _RimColor ("Rim Color", Color) = (0.25, 0.35, 0.45, 1)

        _MainTex ("Main Noise", 2D) = "white" {}
        _FlowTex ("Flow Noise", 2D) = "gray" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}

        _MainTiling ("Main Tiling", Float) = 1
        _FlowTiling ("Flow Tiling", Float) = 1
        _FlowSpeed ("Flow Speed", Vector) = (0.05, -0.03, 0, 0)

        _NormalStrength ("Normal Strength", Range(0, 3)) = 1
        _Smoothness ("Smoothness", Range(0, 1)) = 0.92
        _SpecStrength ("Spec Strength", Range(0, 4)) = 1.5

        _RimPower ("Rim Power", Range(0.1, 8)) = 3
        _RimStrength ("Rim Strength", Range(0, 3)) = 0.7

        _DepthStrength ("Depth Strength", Range(0, 2)) = 0.5
        _FlowDistort ("Flow Distort", Range(0, 1)) = 0.15

        _HeightAmount ("Surface Height Amount", Range(0, 0.2)) = 0.03
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Geometry"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            Cull Back
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_fragment _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct a2v
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float4 tangentOS  : TANGENT;
                float2 uv         : TEXCOORD0;
            };

            struct v2f
            {
                float4 positionCS     : SV_POSITION;
                float3 positionWS     : TEXCOORD0;
                float3 normalWS       : TEXCOORD1;
                float4 tangentWS      : TEXCOORD2;
                float2 uv             : TEXCOORD3;
                float3 viewDirWS      : TEXCOORD4;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _DeepColor;
                float4 _RimColor;

                float _MainTiling;
                float _FlowTiling;
                float4 _FlowSpeed;

                float _NormalStrength;
                float _Smoothness;
                float _SpecStrength;

                float _RimPower;
                float _RimStrength;

                float _DepthStrength;
                float _FlowDistort;
                float _HeightAmount;
            CBUFFER_END

            TEXTURE2D(_MainTex);      SAMPLER(sampler_MainTex);
            TEXTURE2D(_FlowTex);      SAMPLER(sampler_FlowTex);
            TEXTURE2D(_NormalMap);    SAMPLER(sampler_NormalMap);

            float3 UnpackNormalScaleCustom(float4 packedNormal, float scale)
            {
                float3 n = UnpackNormal(packedNormal);
                n.xy *= scale;
                n.z = sqrt(saturate(1.0 - dot(n.xy, n.xy)));
                return n;
            }

            v2f vert(a2v v)
            {
                v2f o;

                float2 flowUV = v.uv * _FlowTiling + _Time.y * _FlowSpeed.xy;
                float flowSample = SAMPLE_TEXTURE2D_LOD(_FlowTex, sampler_FlowTex, flowUV, 0).r;
                float heightOffset = (flowSample * 2.0 - 1.0) * _HeightAmount;

                float3 posOS = v.positionOS.xyz + v.normalOS * heightOffset;

                VertexPositionInputs posInputs = GetVertexPositionInputs(posOS);
                VertexNormalInputs normalInputs = GetVertexNormalInputs(v.normalOS, v.tangentOS);

                o.positionCS = posInputs.positionCS;
                o.positionWS = posInputs.positionWS;
                o.normalWS = normalInputs.normalWS;
                o.tangentWS = float4(normalInputs.tangentWS, v.tangentOS.w);
                o.uv = v.uv;
                o.viewDirWS = GetWorldSpaceViewDir(posInputs.positionWS);

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float3 viewDirWS = normalize(i.viewDirWS);
                float3 normalWS = normalize(i.normalWS);

                float2 baseUV = i.uv * _MainTiling;
                float2 flowUV = i.uv * _FlowTiling;

                float2 flowOffset1 = _Time.y * _FlowSpeed.xy;
                float2 flowOffset2 = _Time.y * float2(-_FlowSpeed.y, _FlowSpeed.x) * 0.7;

                float flowA = SAMPLE_TEXTURE2D(_FlowTex, sampler_FlowTex, flowUV + flowOffset1).r;
                float flowB = SAMPLE_TEXTURE2D(_FlowTex, sampler_FlowTex, flowUV + flowOffset2).r;
                float flowMix = lerp(flowA, flowB, 0.5);

                float2 distortedUV = baseUV + (flowMix * 2.0 - 1.0) * _FlowDistort;

                float noise = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, distortedUV).r;

                float4 packedNormal = SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, distortedUV);
                float3 normalTS = UnpackNormalScaleCustom(packedNormal, _NormalStrength);

                float3 bitangentWS = cross(normalWS, normalize(i.tangentWS.xyz)) * i.tangentWS.w;
                float3x3 TBN = float3x3(normalize(i.tangentWS.xyz), normalize(bitangentWS), normalWS);
                float3 finalNormalWS = normalize(mul(normalTS, TBN));

                Light mainLight = GetMainLight();

                float3 lightDirWS = normalize(mainLight.direction);
                float NdotL = saturate(dot(finalNormalWS, lightDirWS));
                float3 halfDir = normalize(lightDirWS + viewDirWS);
                float NdotH = saturate(dot(finalNormalWS, halfDir));

                float spec = pow(NdotH, lerp(32.0, 256.0, _Smoothness)) * _SpecStrength;

                float fresnel = pow(1.0 - saturate(dot(finalNormalWS, viewDirWS)), _RimPower);
                float3 rim = _RimColor.rgb * fresnel * _RimStrength;

                float depthMask = saturate((1.0 - noise) * _DepthStrength);
                float3 albedo = lerp(_BaseColor.rgb, _DeepColor.rgb, depthMask);

                float3 color = 0;
                color += albedo * (0.12 + NdotL * mainLight.color.rgb);
                color += spec * mainLight.color.rgb;
                color += rim;

                #ifdef _ADDITIONAL_LIGHTS
                uint lightCount = GetAdditionalLightsCount();
                for (uint li = 0; li < lightCount; li++)
                {
                    Light light = GetAdditionalLight(li, i.positionWS);
                    float3 addDir = normalize(light.direction);
                    float addNdotL = saturate(dot(finalNormalWS, addDir));
                    float3 addHalf = normalize(addDir + viewDirWS);
                    float addNdotH = saturate(dot(finalNormalWS, addHalf));
                    float addSpec = pow(addNdotH, lerp(24.0, 128.0, _Smoothness)) * _SpecStrength * 0.5;

                    color += albedo * addNdotL * light.color.rgb * light.distanceAttenuation;
                    color += addSpec * light.color.rgb * light.distanceAttenuation;
                }
                #endif

                return half4(color, 1.0);
            }
            ENDHLSL
        }
    }
}
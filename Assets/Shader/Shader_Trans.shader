Shader "Custom/Shader_Trans"
{
    Properties
    {
        _BaseMap("Base Map", 2D) = "white" {}
        _BaseColor("Base Color", Color) = (1,1,1,0.5)

        _NormalMap("Normal Map", 2D) = "bump" {}
        _NormalScale("Normal Scale", Float) = 1

        _AOMap("AO Map", 2D) = "white" {}
        _MetallicMap("Metallic Map", 2D) = "black" {}
        _RoughnessMap("Roughness Map", 2D) = "white" {}

        _EmissionMap("Emission Map", 2D) = "black" {}
        _EmissionColor("Emission Color", Color) = (0,0,0,0)

        _SpecIntensity("Spec Intensity", Range(0, 8)) = 2
        _SpecMin("Spec Min", Range(0, 1)) = 0.4
        _ShininessMin("Shininess Min", Range(1, 64)) = 4
        _ShininessMax("Shininess Max", Range(8, 256)) = 32
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        // Pass 1: 深度预写入
        Pass
        {
            Name "DepthPrePass"

            Tags { "LightMode"="SRPDefaultUnlit" }

            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct a2v
            {
                float4 positionOS : POSITION;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
            };

            v2f vert(a2v v)
            {
                v2f o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                return 0;
            }
            ENDHLSL
        }

        // Pass 2: 正常半透明渲染
        Pass
        {
            Name "TransparentPass"

            Tags { "LightMode"="UniversalForward" }

            ZWrite Off
            ZTest LEqual
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _FORWARD_PLUS

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_BaseMap);        SAMPLER(sampler_BaseMap);
            TEXTURE2D(_NormalMap);      SAMPLER(sampler_NormalMap);
            TEXTURE2D(_AOMap);          SAMPLER(sampler_AOMap);
            TEXTURE2D(_MetallicMap);    SAMPLER(sampler_MetallicMap);
            TEXTURE2D(_RoughnessMap);   SAMPLER(sampler_RoughnessMap);
            TEXTURE2D(_EmissionMap);    SAMPLER(sampler_EmissionMap);

            CBUFFER_START(UnityPerMaterial)
            float4 _BaseColor;

            float4 _BaseMap_ST;
            float4 _NormalMap_ST;
            float4 _AOMap_ST;
            float4 _MetallicMap_ST;
            float4 _RoughnessMap_ST;
            float4 _EmissionMap_ST;

            float _NormalScale;
            float4 _EmissionColor;

            float _SpecIntensity;
            float _SpecMin;
            float _ShininessMin;
            float _ShininessMax;
            CBUFFER_END

            struct a2v
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float4 tangentOS  : TANGENT;
                float2 uv         : TEXCOORD0;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;

                float3 positionWS : TEXCOORD1;
                float3 normalWS   : TEXCOORD2;
                float3 tangentWS  : TEXCOORD3;
                float3 bitangentWS: TEXCOORD4;
            };

            v2f vert(a2v v)
            {
                v2f o;

                VertexPositionInputs posInput = GetVertexPositionInputs(v.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(v.normalOS, v.tangentOS);

                o.positionCS = posInput.positionCS;
                o.positionWS = posInput.positionWS;

                o.uv = v.uv;

                o.normalWS = normalInput.normalWS;
                o.tangentWS = normalInput.tangentWS;
                o.bitangentWS = normalInput.bitangentWS;

                return o;
            }

            float3 SampleNormal(v2f i)
            {
                float2 uv = TRANSFORM_TEX(i.uv, _NormalMap);

                float3 normalTS = UnpackNormalScale(
                    SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, uv),
                    _NormalScale
                );

                float3x3 TBN = float3x3(
                    normalize(i.tangentWS),
                    normalize(i.bitangentWS),
                    normalize(i.normalWS)
                );

                return normalize(mul(normalTS, TBN));
            }

            half4 frag(v2f i) : SV_Target
            {
                float2 uvBase      = TRANSFORM_TEX(i.uv, _BaseMap);
                float2 uvAO        = TRANSFORM_TEX(i.uv, _AOMap);
                float2 uvMetallic  = TRANSFORM_TEX(i.uv, _MetallicMap);
                float2 uvRoughness = TRANSFORM_TEX(i.uv, _RoughnessMap);
                float2 uvEmission  = TRANSFORM_TEX(i.uv, _EmissionMap);

                half4 base = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uvBase) * _BaseColor;

                float3 normalWS = SampleNormal(i);
                float3 viewDir = normalize(GetCameraPositionWS() - i.positionWS);

                float ao = SAMPLE_TEXTURE2D(_AOMap, sampler_AOMap, uvAO).r;
                float metallic = SAMPLE_TEXTURE2D(_MetallicMap, sampler_MetallicMap, uvMetallic).r;
                float roughness = SAMPLE_TEXTURE2D(_RoughnessMap, sampler_RoughnessMap, uvRoughness).r;

                float shininess = lerp(_ShininessMax, _ShininessMin, roughness);
                float specMask = lerp(_SpecMin, 1.0, metallic);

                float3 diffuseSum = 0;
                float3 specularSum = 0;

                // ===== Main Light =====
                Light mainLight = GetMainLight();
                {
                    float3 lightDir = normalize(mainLight.direction);
                    float3 halfDir = normalize(lightDir + viewDir);

                    float NdotL = saturate(dot(normalWS, lightDir));
                    float NdotH = saturate(dot(normalWS, halfDir));

                    float3 diffuse = base.rgb * NdotL * ao * mainLight.color;
                    float spec = pow(NdotH, shininess) * specMask * _SpecIntensity;
                    float3 specular = spec * mainLight.color;

                    diffuseSum += diffuse;
                    specularSum += specular;
                }

                // ===== Additional Lights =====
                #if defined(_ADDITIONAL_LIGHTS)
                uint additionalLightsCount = GetAdditionalLightsCount();
                for (uint lightIndex = 0u; lightIndex < additionalLightsCount; ++lightIndex)
                {
                    Light light = GetAdditionalLight(lightIndex, i.positionWS);

                    float3 lightDir = normalize(light.direction);
                    float3 halfDir = normalize(lightDir + viewDir);

                    float NdotL = saturate(dot(normalWS, lightDir));
                    float NdotH = saturate(dot(normalWS, halfDir));

                    float atten = light.distanceAttenuation * light.shadowAttenuation;

                    float3 diffuse = base.rgb * NdotL * ao * light.color * atten;
                    float spec = pow(NdotH, shininess) * specMask * _SpecIntensity * atten;
                    float3 specular = spec * light.color;

                    diffuseSum += diffuse;
                    specularSum += specular;
                }
                #endif

                // ===== IBL / Reflection Probe ===== ***反射探针
                float3 reflDir = reflect(-viewDir, normalWS);

                // roughness -> perceptualRoughness（URP 用这个）
                float perceptualRoughness = roughness;

                // Unity 标准做法
                float3 indirectSpec = GlossyEnvironmentReflection(
                    reflDir,
                    perceptualRoughness,
                    1.0 // occlusion，可以先写1
                );

                // 金属度影响反射强度
                float3 iblSpec = indirectSpec * lerp(_SpecMin, 1.0, metallic) * _SpecIntensity;

                float3 emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, uvEmission).rgb * _EmissionColor.rgb;

                float3 color = diffuseSum + specularSum + iblSpec + emission;

                // fresnel
                float F0 = 0.95;
                float alpha = F0 + (1-F0) * pow((1 - dot(viewDir, normalWS)), 5);

                return float4(color, alpha);
            }
            ENDHLSL
        }
    }
}
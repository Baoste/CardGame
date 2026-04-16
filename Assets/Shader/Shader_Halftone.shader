Shader "Unlit/Shader_Halftone"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _HalftoneColor ("Halftone Color", Color) = (1,1,1,1)
        _Radius ("Radius", float) = 0.5
        _Freq ("Frequency of halftone", float) = 5
        _LightsCount ("Lights Count", float) = 5
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
        }
        LOD 100

        Pass
        {
            Name "Halftone"
            ZTest Always
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // 额外光
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            TEXTURE2D_X(_BlitTexture);
            SAMPLER(sampler_BlitTexture);

            TEXTURE2D(_CameraNormalsTexture);
            SAMPLER(sampler_CameraNormalsTexture);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _HalftoneColor;
                float4 _BaseMap_ST;
                float _Radius;
                float _Freq;
                float _LightsCount;
            CBUFFER_END

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            struct a2v
            {
                uint vertexID : SV_VertexID;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
            };
            
            v2f vert (a2v v)
            {
                v2f o;

                float2 uv = float2((v.vertexID << 1) & 2, v.vertexID & 2);
                o.positionCS = float4(uv * 2.0 - 1.0, 0.0, 1.0);
                o.uv = uv;

            #if UNITY_UV_STARTS_AT_TOP
                o.uv.y = 1.0 - o.uv.y;
            #endif

                return o;
            }

            float3 RGB2OKLAB(float3 c)
            {
                float l = 0.41222147*c.r + 0.53633254*c.g + 0.05144599*c.b;
                float m = 0.21190350*c.r + 0.68069955*c.g + 0.10739696*c.b;
                float s = 0.08830246*c.r + 0.28171884*c.g + 0.62997870*c.b;
                float3 lms = pow(float3(l,m,s), 1.0/3.0);
                return float3(
                    0.21045426*lms.x + 0.79361778*lms.y - 0.00407205*lms.z,
                    1.97799850*lms.x - 2.42859221*lms.y + 0.45059371*lms.z,
                    0.02590404*lms.x + 0.78277177*lms.y - 0.80867577*lms.z);
            }

            float3 OKLAB2RGB(float3 lab)
            {
                float L = lab.x, a = lab.y, b = lab.z;

                float l_ = L + 0.3963377774*a + 0.2158037573*b;
                float m_ = L - 0.1055613458*a - 0.0638541728*b;
                float s_ = L - 0.0894841775*a - 1.2914855480*b;

                float l = l_*l_*l_;   // cube
                float m = m_*m_*m_;
                float s = s_*s_*s_;

                float3 rgb;
                rgb.r =  4.0767416621*l - 3.3077115913*m + 0.2309699292*s;
                rgb.g = -1.2684380046*l + 2.6097574011*m - 0.3413193965*s;
                rgb.b = -0.0041960863*l - 0.7034186147*m + 1.7076147010*s;
                return rgb;
            }

            float3 GetWorldPosFromScreenUV(float2 screenUV)
            {
                real depth = SampleSceneDepth(screenUV);
                // 不同图形 API 的 NDC Z 范围不同，Unity 官方示例就是这样处理
            #if UNITY_REVERSED_Z
                float z = depth;
            #else
                float z = lerp(UNITY_NEAR_CLIP_VALUE, 1.0, depth);
            #endif
                float3 positionWS = ComputeWorldSpacePosition(screenUV, z, UNITY_MATRIX_I_VP);
                return positionWS;
            }

            float3 CalcHalftone(float2 uv, float3 halftoneColor, float3 bgColor, float radius)
            {
                // rotate 45 degree
                float angle = 0.78539816339;
                float s = sin(angle);
                float c = cos(angle);
                uv -= float2(0.5, 0.5);
                uv = float2(uv.x * c - uv.y * s, uv.x * s + uv.y * c);
                uv += float2(0.5, 0.5);

                float2 uv2 = 2 * frac(uv * _Freq) - 1;
                //uv2.y = 0;

                float dist = length(uv2);
                float width = 0.01;
                float ss = smoothstep(radius-width, radius+width, dist);
                halftoneColor = OKLAB2RGB(lerp(RGB2OKLAB(bgColor), RGB2OKLAB(halftoneColor), radius/4));
                //halftoneColor = lerp(bgColor, halftoneColor, radius/16);
                return lerp(halftoneColor, bgColor, ss);
            }

            half4 frag (v2f i) : SV_Target
            {
                float3 normalWS = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, i.uv);
                float3 worldPos = GetWorldPosFromScreenUV(i.uv);
                float3 absN = abs(normalWS);

                float2 sample_uv;
                if (absN.x > absN.y && absN.x > absN.z)
                {
                    // X dominant
                    sample_uv.xy = worldPos.yz;
                }
                else if (absN.y > absN.z)
                {
                    // Y dominant
                    sample_uv.xy = worldPos.xz;
                }
                else
                {
                    // Z dominant
                    sample_uv.xy = worldPos.xy;
                }

                float3 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, i.uv).rgb;

                // ===== 额外光（点光 / 聚光 / 非主方向光）=====
                float all_atten = 0;
                half3 all_lightColor = half3(0,0,0);

                uint excludeMask = 1u << 1;   // 过滤掉 Rendering Layer n
                for (uint i = 0; i < _LightsCount; i++)
                {
                    Light light = GetAdditionalPerObjectLight(i, worldPos);
                    if ((light.layerMask & excludeMask) != 0u)
                        continue;

                    half3 L = normalize(light.direction);
                    half3 lightColor = light.color;

                    //float3 V = normalize(_WorldSpaceCameraPos - worldPos);
                    //float3 H = normalize(L + V);
                    //float NdotH = saturate(dot(normalWS, H));
                    //float specPower = lerp(1.0, 128.0, smoothness);
                    //float specWeight = pow(NdotH, specPower);

                    half NdotL = saturate(dot(normalWS, L));
                    float atten = light.distanceAttenuation * light.shadowAttenuation;
                    float weight = atten * NdotL;
                    all_lightColor += lightColor * weight;
                    all_atten += weight;
                }

                float radius = smoothstep(0.0, 0.12, all_atten) * _Radius;
                half4 col = half4(CalcHalftone(sample_uv, all_lightColor * _HalftoneColor, color, radius), 1);
                return col;
            }

            ENDHLSL
        }
    }
}

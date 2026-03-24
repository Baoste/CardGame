Shader "Custom/DiskShader"
{
    Properties
    {
        _Mask("Mask", 2D) = "white" {}
        _Mask2("Mask2", 2D) = "white" {}

        _BaseColor ("Base Color", Color) = (0.8, 0.8, 0.8, 1)

        _Metallic ("Metallic", Range(0,1)) = 1
        _Roughness ("Roughness", Range(0.01,1)) = 0.25
        _Anisotropy ("Anisotropy", Range(0,1)) = 0.85

        _DiskCenterOS ("Disk Center OS", Vector) = (0,0,0,1)
        _RadialNoiseScale ("Radial Noise Scale", Float) = 80
        _RadialNoiseStrength ("Radial Noise Strength", Range(0,1)) = 0.08
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Blend OneMinusSrcAlpha SrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct a2v
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
                float3 positionOS : TEXCOORD2;
                float2 uv         : TEXCOORD3;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float _Metallic;
                float _Roughness;
                float _Anisotropy;

                float4 _DiskCenterOS;
                float _RadialNoiseScale;
                float _RadialNoiseStrength;
            CBUFFER_END

            TEXTURE2D(_Mask);
            SAMPLER(sampler_Mask);
            TEXTURE2D(_Mask2);
            SAMPLER(sampler_Mask2);

            v2f vert(a2v v)
            {
                v2f o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.positionOS = v.positionOS.xyz;
                o.uv = v.uv;
                return o;
            }

            //基于位置的伪随机数生成器，适合在 shader 中创建稳定的噪声模式。
            float Hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            //菲涅尔
            float3 FresnelSchlick(float cosTheta, float3 F0)
            {
                return F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);
            }

            float D_GGX_Aniso(float NdotH, float TdotH, float BdotH, float at, float ab)
            {
                float a2 = (TdotH * TdotH) / (at * at) + (BdotH * BdotH) / (ab * ab) + NdotH * NdotH;
                return 1.0 / max(PI * at * ab * a2 * a2, 1e-5);
            }

            float G_SmithGGX(float NdotV, float alpha)
            {
                float a = alpha * alpha;
                float k = sqrt(a + (1.0 - a) * NdotV * NdotV);
                return 2.0 * NdotV / max(NdotV + k, 1e-5);
            }

            float3 test(float3 N, v2f i)
            {
                // float3 N = normalize(i.normalWS);
                float3 V = normalize(_WorldSpaceCameraPos - i.positionWS);

                Light mainLight = GetMainLight();
                mainLight.direction.y = -mainLight.direction.y;
                // Light mainLight = GetAdditionalLight(0, i.positionWS);
                float3 L = normalize(mainLight.direction);
                float3 H = normalize(V + L);

                // ===== 圆盘局部方向 =====
                float2 localXZ = i.positionOS.xz - _DiskCenterOS.xz;
                float lenXZ = length(localXZ);

                // 防止中心点出 NaN
                float2 radial2D = (lenXZ > 1e-5) ? localXZ / lenXZ : float2(1, 0);

                // 圆周方向（圆盘表面假设主要在 XZ 平面）
                float3 radialDirOS  = normalize(float3(radial2D.x, 0, radial2D.y));
                float3 tangentDirOS = normalize(float3(-radial2D.y, 0, radial2D.x));

                float3 T = normalize(TransformObjectToWorldDir(tangentDirOS));
                float3 B = normalize(cross(N, T));
                T = normalize(cross(B, N));

                // ===== 拉丝扰动 =====
                float ringNoise = Hash21(floor(radial2D * _RadialNoiseScale + lenXZ * _RadialNoiseScale));
                float roughnessJitter = lerp(1.0 - _RadialNoiseStrength, 1.0 + _RadialNoiseStrength, ringNoise);

                float roughness = saturate(_Roughness * roughnessJitter);
                roughness = max(roughness, 0.02);

                // ===== 各向异性粗糙度 =====
                // 沿 T 方向更光滑，高光会沿 T 拉长
                float at = max(0.02, roughness * (1.0 - _Anisotropy));
                float ab = max(0.02, roughness * (1.0 + _Anisotropy));

                float NdotL = saturate(dot(N, L));
                float NdotV = saturate(dot(N, V));
                float NdotH = saturate(dot(N, H));
                float VdotH = saturate(dot(V, H));
                float TdotH = dot(T, H);
                float BdotH = dot(B, H);

                float3 albedo = _BaseColor.rgb;

                // 金属盘通常就是金属
                float3 F0 = lerp(float3(0.04, 0.04, 0.04), albedo, _Metallic);

                float D = D_GGX_Aniso(NdotH, TdotH, BdotH, at, ab);
                float Gv = G_SmithGGX(NdotV, roughness);
                float Gl = G_SmithGGX(NdotL, roughness);
                float G = Gv * Gl;
                float3 F = FresnelSchlick(VdotH, F0);

                float3 specular = (D * G * F) / max(4.0 * NdotV * NdotL, 1e-5);

                float3 kd = (1.0 - F) * (1.0 - _Metallic);
                float3 diffuse = kd * albedo / PI;

                float3 color = (diffuse + specular) * mainLight.color * NdotL;
                return color;
            }

            half4 frag(v2f i) : SV_Target
            {
                float mask1 = 1 - _Mask.Sample(sampler_Mask, i.uv).r;
                float mask2 = 1 - _Mask2.Sample(sampler_Mask2, i.uv).r;
                float mask = mask1 * mask2;
                if (mask > 0.5)
                {
                    return half4(1, 1, 1, mask);
                }
                float3 Nr = normalize(i.normalWS);
                float3 Ng = normalize(i.normalWS);
                float3 Nb = normalize(i.normalWS);
                float delta = 0.06;
                Nr.x -= delta;
                Nb.x += delta;
                // 很淡的环境底色，避免纯黑
                float3 colorr= test(Nr, i);
                float3 colorg= test(Ng, i);
                float3 colorb= test(Nb, i);
                float3 color = float3(colorr.r, colorg.g, colorb.b);
                float3 albedo = _BaseColor.rgb;
                color += albedo * 0.03;
                return half4(color, mask);
            }

            ENDHLSL
        }
    }
}

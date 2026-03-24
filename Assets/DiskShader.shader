Shader "Custom/DiskShader"
{
    Properties
    {
        _Mask("Mask", 2D) = "white" {}

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
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }

        Pass
        {
            Name "Forward"
            Tags { "LightMode"="UniversalForward" }

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

            struct DiskPreData
            {
                float3 V;
                float3 L;
                float3 H;
                float3 lightColor;

                float3 tangentWSBase;

                float roughness;
                float at;
                float ab;

                float3 albedo;
                float3 F0;
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

            float Hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

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

            DiskPreData PrepareDiskData(v2f i)
            {
                DiskPreData o;

                o.V = normalize(_WorldSpaceCameraPos - i.positionWS);

                Light mainLight = GetMainLight();
                o.L = normalize(mainLight.direction);
                o.H = normalize(o.V + o.L);
                o.lightColor = mainLight.color;

                float2 localXZ = i.positionOS.xz - _DiskCenterOS.xz;
                float lenXZ = length(localXZ);
                float2 radial2D = (lenXZ > 1e-5) ? localXZ / lenXZ : float2(1, 0);

                float3 tangentDirOS = float3(-radial2D.y, 0, radial2D.x);
                o.tangentWSBase = normalize(TransformObjectToWorldDir(tangentDirOS));

                float ringNoise = Hash21(floor(radial2D * _RadialNoiseScale + lenXZ * _RadialNoiseScale));
                float roughnessJitter = lerp(1.0 - _RadialNoiseStrength, 1.0 + _RadialNoiseStrength, ringNoise);

                o.roughness = max(saturate(_Roughness * roughnessJitter), 0.02);

                o.at = max(0.02, o.roughness * (1.0 - _Anisotropy));
                o.ab = max(0.02, o.roughness * (1.0 + _Anisotropy));

                o.albedo = _BaseColor.rgb;
                o.F0 = lerp(float3(0.04, 0.04, 0.04), o.albedo, _Metallic);

                return o;
            }

            float3 EvalDiskLighting(float3 N, DiskPreData pre)
            {
                N = normalize(N);

                float3 T = pre.tangentWSBase;
                float3 B = normalize(cross(N, T));
                T = normalize(cross(B, N));

                float NdotL = saturate(dot(N, pre.L));
                float NdotV = saturate(dot(N, pre.V));
                float NdotH = saturate(dot(N, pre.H));
                float VdotH = saturate(dot(pre.V, pre.H));
                float TdotH = dot(T, pre.H);
                float BdotH = dot(B, pre.H);

                float D = D_GGX_Aniso(NdotH, TdotH, BdotH, pre.at, pre.ab);
                float Gv = G_SmithGGX(NdotV, pre.roughness);
                float Gl = G_SmithGGX(NdotL, pre.roughness);
                float G = Gv * Gl;
                float3 F = FresnelSchlick(VdotH, pre.F0);

                float3 specular = (D * G * F) / max(4.0 * NdotV * NdotL, 1e-5);
                float3 kd = (1.0 - F) * (1.0 - _Metallic);
                float3 diffuse = kd * pre.albedo / PI;

                return (diffuse + specular) * pre.lightColor * NdotL;
            }

            half4 frag(v2f i) : SV_Target
            {
                half mask = SAMPLE_TEXTURE2D(_Mask, sampler_Mask, i.uv).r;

                float3 N0 = normalize(i.normalWS);
                float3 Nr = N0;
                float3 Ng = N0;
                float3 Nb = N0;

                float delta = 0.06;
                Nr.x -= delta;
                Nb.x += delta;

                DiskPreData pre = PrepareDiskData(i);

                float3 colorr = EvalDiskLighting(Nr, pre);
                float3 colorg = EvalDiskLighting(Ng, pre);
                float3 colorb = EvalDiskLighting(Nb, pre);

                float3 color = float3(colorr.r, colorg.g, colorb.b) * (1.0 - mask);
                color += pre.albedo * 0.03;

                return half4(color, 1);
            }

            ENDHLSL
        }
    }
}
Shader "Unlit/Shader_SmokeOrbButton_UI"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        _Color ("Tint", Color) = (1,1,1,1)

        _CoreColor ("Core Color", Color) = (1,1,1,1)
        _SmokeColor ("Smoke Color", Color) = (0.42,0.52,0.75,1)

        _CoreRadius ("Core Radius", Range(0.01, 0.5)) = 0.10
        _GlowRadius ("Glow Radius", Range(0.05, 1.2)) = 0.55

        _SmokeInner ("Smoke Inner", Range(0.0, 1.0)) = 0.08
        _SmokeOuter ("Smoke Outer", Range(0.1, 1.8)) = 0.95
        _SmokeStrength ("Smoke Strength", Range(0.0, 3.0)) = 0.85

        _NoiseScale ("Noise Scale", Range(0.5, 12.0)) = 3.2
        _NoiseSpeed ("Noise Speed", Range(0.0, 3.0)) = 0.35
        _RandomSeed ("Random Seed", Range(0.0, 1000.0)) = 0.0

        _SwirlStrength ("Swirl Strength", Range(0.0, 8.0)) = 2.5
        _WispSharpness ("Wisp Sharpness", Range(0.5, 5.0)) = 2.2
        _EdgeBreakup ("Edge Breakup", Range(0.0, 1.0)) = 0.75

        _Aspect ("Aspect", Range(0.2, 5.0)) = 1.0
        _Alpha ("Alpha", Range(0.0, 1.0)) = 1.0

        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
        [HideInInspector] _ClipRect ("Clip Rect", Vector) = (-32767, -32767, 32767, 32767)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend One OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "WispySmokeOrb"

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _CoreColor;
                float4 _SmokeColor;

                float _CoreRadius;
                float _GlowRadius;

                float _SmokeInner;
                float _SmokeOuter;
                float _SmokeStrength;

                float _NoiseScale;
                float _NoiseSpeed;
                float _RandomSeed;

                float _SwirlStrength;
                float _WispSharpness;
                float _EdgeBreakup;

                float _Aspect;
                float _Alpha;

                float4 _ClipRect;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color      : COLOR;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 color      : COLOR;
                float2 uv         : TEXCOORD0;
                float2 localPos   : TEXCOORD1;
            };

            static const float3x3 NOISE_M = float3x3(
                 0.00,  0.80,  0.60,
                -0.80,  0.36, -0.48,
                -0.60, -0.48,  0.64
            );

            float Hash(float n)
            {
                return frac(sin(n) * 43758.5453123);
            }

            float Noise3D(float3 x)
            {
                float3 p = floor(x);
                float3 f = frac(x);

                f = f * f * (3.0 - 2.0 * f);

                float n = p.x + p.y * 57.0 + 113.0 * p.z;

                float res =
                    lerp(
                        lerp(
                            lerp(Hash(n +   0.0), Hash(n +   1.0), f.x),
                            lerp(Hash(n +  57.0), Hash(n +  58.0), f.x),
                            f.y
                        ),
                        lerp(
                            lerp(Hash(n + 113.0), Hash(n + 114.0), f.x),
                            lerp(Hash(n + 170.0), Hash(n + 171.0), f.x),
                            f.y
                        ),
                        f.z
                    );

                return res;
            }

            float FBM(float3 p)
            {
                float f = 0.0;

                f += 0.5000 * Noise3D(p);
                p = mul(NOISE_M, p) * 2.02;

                f += 0.2500 * Noise3D(p);
                p = mul(NOISE_M, p) * 2.03;

                f += 0.1250 * Noise3D(p);
                p = mul(NOISE_M, p) * 2.01;

                f += 0.0625 * Noise3D(p);

                return f;
            }

            float GetUIClipAlpha(float2 localPos)
            {
                #if defined(UNITY_UI_CLIP_RECT)
                    float2 insideMin = step(_ClipRect.xy, localPos);
                    float2 insideMax = step(localPos, _ClipRect.zw);
                    return insideMin.x * insideMin.y * insideMax.x * insideMax.y;
                #else
                    return 1.0;
                #endif
            }

            Varyings Vert(Attributes input)
            {
                Varyings output;

                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.color = input.color * _Color;
                output.localPos = input.positionOS.xy;

                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                half4 spriteTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);

                float2 p = input.uv * 2.0 - 1.0;
                p.x *= _Aspect;

                float r = length(p);
                float2 dir = p / max(r, 0.0001);
                float2 tangent = float2(-dir.y, dir.x);

                float time = _Time.y * _NoiseSpeed;

                float seed = _RandomSeed;

                float3 seedOffset = float3(
                    seed * 12.9898,
                    seed * 78.233,
                    seed * 37.719
                );

                // 中心白点
                float core = 1.0 - smoothstep(_CoreRadius * 0.75, _CoreRadius, r);

                // 中心光辉
                float glow = 1.0 - smoothstep(_CoreRadius, _GlowRadius, r);
                glow = pow(saturate(glow), 1.15);

                // 让烟雾有旋转流动，但不是完整球面旋转
                float baseNoise = FBM(float3(p * _NoiseScale, time));
                float swirl = time * 1.5 + baseNoise * 2.2 + (1.0 - r) * _SwirlStrength;

                float2 flowP = p;
                flowP += tangent * sin(swirl + r * 5.0) * 0.18;
                flowP += dir * sin(time * 1.7 + r * 8.0) * 0.05;

                // 两层噪声形成烟丝
                float n1 = FBM(float3(flowP * _NoiseScale, time) + seedOffset);
                float n2 = FBM(float3((flowP + tangent * 0.35) * _NoiseScale * 2.1, time + 17.3) + seedOffset);

                float wisps = saturate(n1 * 0.85 + n2 * 0.55 - 0.48);
                wisps = pow(wisps, _WispSharpness);

                // 打碎外轮廓：不要形成标准圆
                float edgeNoise = FBM(float3(dir * _NoiseScale * 1.4, r * 2.5 + time) + seedOffset);
                float brokenOuter = _SmokeOuter * lerp(
                    1.0 - _EdgeBreakup * 0.45,
                    1.0 + _EdgeBreakup * 0.35,
                    edgeNoise
                );

                float outerMask = 1.0 - smoothstep(brokenOuter * 0.72, brokenOuter, r);
                float innerMask = smoothstep(_SmokeInner * 0.35, _SmokeInner, r);

                // 角度方向的不均匀分布，继续削弱球形感
                float angle = atan2(p.y, p.x);
                float lobe = FBM(float3(cos(angle) * 1.8, sin(angle) * 1.8, time * 0.7) + seedOffset);
                float lobeMask = lerp(0.35, 1.25, lobe);

                // 径向衰减：让雾自然散开，而不是一个完整球壳
                float radialFade = exp(-r * 0.95);

                float smokeAlpha = wisps;
                smokeAlpha *= outerMask;
                smokeAlpha *= innerMask;
                smokeAlpha *= radialFade;
                smokeAlpha *= lobeMask;
                smokeAlpha *= _SmokeStrength;

                smokeAlpha = saturate(smokeAlpha);

                // 越靠近中心，烟雾越被白光吞进去
                float smokeToCore = saturate(glow * 1.5 + core);
                float3 fusedSmokeColor = lerp(_SmokeColor.rgb, _CoreColor.rgb, smokeToCore);

                float alpha = 0.0;
                alpha += core;
                alpha += glow * 0.42;
                alpha += smokeAlpha * 0.85;
                alpha = saturate(alpha);

                alpha *= _Alpha;
                alpha *= input.color.a;
                alpha *= spriteTex.a;
                alpha *= GetUIClipAlpha(input.localPos);

                float3 color = 0.0;
                color += _CoreColor.rgb * core * 2.2;
                color += _CoreColor.rgb * glow * 0.95;
                color += fusedSmokeColor * smokeAlpha * 1.15;

                color *= input.color.rgb;

                return half4(color * alpha, alpha);
            }

            ENDHLSL
        }
    }
}
Shader "Unlit/Shader_MusicRhythm"
{
    Properties
    {
        _BaseMap ("Texture", 2D) = "white" {}
        _NoiseMap ("Noise", 2D) = "white" {}
        _PeaceMap ("Peace", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _HalftoneColor ("Halftone Color", Color) = (1,1,1,1)
        _Radius ("Radius", float) = 0.5
        _MoveSpeed ("Move Speed", float) = 0.5
        _Interval ("Interval", float) = 0.1
        _FreqX ("Frequency X of halftone", float) = 5
        _FreqY ("Frequency Y of halftone", float) = 5
        _MaskThreshold ("Mask threshold", float) = 0.5
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalRenderPipeline"
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha One
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _HalftoneColor;
                float4 _BaseMap_ST;
                float _Radius;
                float _MoveSpeed;
                float _Interval;
                float _FreqX;
                float _FreqY;
                float _MaskThreshold;
            CBUFFER_END

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);
            TEXTURE2D(_NoiseMap);
            SAMPLER(sampler_NoiseMap);
            TEXTURE2D(_PeaceMap);
            SAMPLER(sampler_PeaceMap);

            struct a2v
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
            };


            float hash(float n)
            {
                return frac(sin(n) * 43758.5453123);
            }

            float RandomStep(float time, float interval)
            {
                float step = floor(time / interval);
                return hash(step);
            }

            float4 CalcHalftone(float2 uv, float4 halftoneColor, float4 bgColor, float radius)
            {
                uv.x += _Time.y * _MoveSpeed;

                float2 uv2X = 2 * frac(uv * _FreqX) - 1;
                float2 uv2Y = 2 * frac(uv * _FreqY) - 1;

                float distX = length(uv2X.x);
                float distY = length(uv2Y.y);
                float ss = step(radius, distX);
                ss *= step(radius, distY);

                float2 tuv = uv;
                float random = RandomStep(_Time.y, _Interval * 0.5);
                tuv.x += (floor(random * _FreqX) + 0.5) / _FreqX;
                //tuv.x += (floor(_Time.y * _MoveSpeed * 20)+0.5) / _FreqX;
                //tuv.x += 0.5 / _FreqX;
                tuv.y += 0.5 / _FreqY;
                tuv.x = floor(tuv.x * _FreqX) / _FreqX;
                tuv.y = floor(tuv.y * _FreqY) / _FreqY;

                float mask1 = SAMPLE_TEXTURE2D(_NoiseMap, sampler_NoiseMap, tuv).r;
                float mask2 = SAMPLE_TEXTURE2D(_PeaceMap, sampler_PeaceMap, tuv).r;

                //float t = (1.0 + sin(_Time.y * 2 * 3.14159265 / _Interval)) * 0.5;
                float t = abs(frac(_Time.y / _Interval + _Interval/2) * 2 - 1);
                t = pow(t, 0.4);
                float mask = lerp(mask2, mask1, t);

                float m = step(_MaskThreshold, ss * mask);
                return lerp(bgColor, halftoneColor+half4(0,0.5,1,1)*5, m);
            }

            v2f vert (a2v v)
            {
                v2f o;

                VertexPositionInputs posInputs = GetVertexPositionInputs(v.positionOS);
                VertexNormalInputs nrmInputs = GetVertexNormalInputs(v.normalOS);

                o.positionCS = posInputs.positionCS;
                o.normalWS = nrmInputs.normalWS;
                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                Light mainLight = GetMainLight();

                half4 col = half4(CalcHalftone(i.uv, _HalftoneColor, _BaseColor, _Radius));
                col.a = 0.8;
                return col;
            }

            ENDHLSL
        }
    }
}

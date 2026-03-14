Shader "Unlit/Shader_Halftone"
{
    Properties
    {
        _BaseMap ("Texture", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _HalftoneColor ("Halftone Color", Color) = (1,1,1,1)
        _Radius ("Radius", float) = 0.5
        _Freq ("Frequency of halftone", float) = 5
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalRenderPipeline"
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

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
                float _Freq;
            CBUFFER_END

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

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
                // uv2.y = 0;

                float dist = length(uv2);
                float width = 0.01;
                float ss = smoothstep(radius-width, radius+width, dist);
                return lerp(halftoneColor, bgColor, ss);
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

                half4 col = half4(CalcHalftone(i.uv, _HalftoneColor, _BaseColor, _Radius), 1);
                return col;
            }

            ENDHLSL
        }
    }
}

Shader "Custom/Shader_Cartoon"
{
        Properties
    {
        _BaseMap ("Texture", 2D) = "white" {}
        [HDR] _BaseColor ("Base Color", Color) = (1,1,1,1)
        _Controller ("Controller", Range(0,1)) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseColor;
                float4 _BaseMap_ST;
                float _Controller;
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
                float3 positionWS : TEXCOORD2;
            };

            v2f vert(a2v v)
            {
                v2f o;

                VertexPositionInputs posInputs = GetVertexPositionInputs(v.positionOS.xyz);
                VertexNormalInputs nrmInputs = GetVertexNormalInputs(v.normalOS);

                o.positionCS = posInputs.positionCS;
                o.positionWS = posInputs.positionWS;
                o.normalWS = nrmInputs.normalWS;
                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float3 normalWS = normalize(i.normalWS);
                float3 viewDirWS = normalize(_WorldSpaceCameraPos - i.positionWS);

                float fresnel = pow(1.0 - saturate(dot(normalWS, viewDirWS)), 0.8);

                half4 fresnelCol = _BaseColor;
                fresnelCol.rgb *= fresnel;
                fresnelCol.a *= fresnel;

                half4 baseCol = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);

                half4 col = lerp(fresnelCol, baseCol, _Controller);
                return col;
            }

            ENDHLSL
        }
    }
}

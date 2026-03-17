//
//  OutlineFill.shader
//  QuickOutline
//
//  Created by Chris Nolet on 2/21/18.
//  Copyright © 2018 Chris Nolet. All rights reserved.
//

Shader "Custom/Outline Fill" 
{
    Properties 
    {
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest("ZTest", Float) = 0

        [HDR] _OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineWidth("Outline Width", Range(0, 10)) = 2
        _OutlineWaveTex("Outline Wave Tex", 2D) = "white" {}
        _WaveScroll("Wave Scroll", Vector) = (0.1, 0.0, 0, 0)
    }

    SubShader 
    {
        Tags 
        {
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent+110"
            "RenderType" = "Transparent"
            "DisableBatching" = "True"
        }

        Pass 
        {
            Name "Fill"
            Cull Off
            ZTest [_ZTest]
            ZWrite Off
            Blend SrcAlpha One
            ColorMask RGB

            Stencil {
                Ref 1
                Comp NotEqual
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct a2v
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float2 uv           : TEXCOORD0;
                float3 smoothNormal : TEXCOORD3;
            };

            struct v2f
            {
                float4 positionCS : SV_POSITION;
                half4 color       : COLOR;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _OutlineColor;
                float _OutlineWidth;
                float4 _WaveScroll;
            CBUFFER_END

            TEXTURE2D(_OutlineWaveTex);
            SAMPLER(sampler_OutlineWaveTex);
            float4 _OutlineWaveTex_ST;

            v2f vert(a2v v)
            {
                v2f o;

                float3 normalOS = any(v.smoothNormal) ? v.smoothNormal : v.normalOS;

                float3 positionVS = mul(UNITY_MATRIX_MV, v.positionOS).xyz;

                // object normal -> world normal -> view normal
                float3 normalWS = TransformObjectToWorldNormal(normalOS);
                float3 normalVS = normalize(TransformWorldToViewDir(normalWS, false));

                // outline wave
                //float2 uv = TRANSFORM_TEX(v.uv, _OutlineWaveTex);
                //float2 uvScrolled = uv + _Time.y * _WaveScroll.xy;
                // 用XZ平面（常见）
                float2 uv = v.positionOS.xy;
                float2 uvScrolled = uv + _Time.y * _WaveScroll.xy;

                // 采样
                float wave = SAMPLE_TEXTURE2D_LOD(
                    _OutlineWaveTex,
                    sampler_OutlineWaveTex,
                    uvScrolled,
                    0
                ).r;
                wave = wave * 2.0 - 1.0;   // [-1,1]
                wave = smoothstep(-1.0, 1.0, wave);
                float width = _OutlineWidth + wave * 10;
                width = max(0.0, width);

                positionVS += normalVS * (-positionVS.z) * (width / 1000.0);

                o.positionCS = TransformWViewToHClip(positionVS);

                //float4 originalPosCS = TransformObjectToHClip(v.positionOS);
                //float4 outlinePosCS = o.positionCS;
                //float2 originalNDC = originalPosCS.xy / originalPosCS.w;
                //float2 outlineNDC  = outlinePosCS.xy / outlinePosCS.w;
                //float delta = length(outlineNDC - originalNDC);
                ////delta *= _ScreenParams.y;
                //float fade = saturate(delta * 60);
                //half4 col = _OutlineColor;
                //col.a = lerp(1, 0, fade);
                ////o.color = half4(col.a,col.a,col.a,1);
                //o.color = col;
                o.color = _OutlineColor;

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                return i.color;
            }
            ENDHLSL
        }
    }
}

Shader "Custom/Shader_ComputerScreen"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "black" {}
        _StandbyTex ("Standby Texture", 2D) = "black" {}

        _isStandby ("Is Standby", Int) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }

        Pass
        {
            Name "ForwardLit"

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_StandbyTex);
            SAMPLER(sampler_StandbyTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _StandbyTex_ST;

                int _isStandby;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv          : TEXCOORD0;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                return output;
            }

            half Random(float x)
            {
                return frac(sin(x * 12.9898) * 43758.5453);
            }

            half4 frag(Varyings input) : SV_Target
            {
                if (_isStandby == 1)
                {
                    float t = _Time.y;

                    // 把时间切成一段一段
                    float interval = floor(t * 2);    //数字越大检查闪烁频率越高

                    // 每段时间生成一个随机值
                    float r = Random(interval);     

                    // 只有少数时间段允许闪
                    float canFlash = step(0.7, r);     //数字越大闪烁概率越低

                    // 在允许闪的时间段内，快速闪几下
                    float flicker = step(0.55, Random(floor(t * 50.0)));    //第一个参数控制在闪烁期间，standby 贴图出现的比例。第二个参数越大，闪得越碎

                    float flash = canFlash * flicker;

                    half4 standbyCol = SAMPLE_TEXTURE2D(_StandbyTex, sampler_StandbyTex, input.uv);
                    half4 blackCol = half4(0, 0, 0, 1);

                    return lerp(blackCol, standbyCol, flash);
                }
                else
                {
                    half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                    return col;
                }
            }

            ENDHLSL
        }
    }

    FallBack "Diffuse"
}

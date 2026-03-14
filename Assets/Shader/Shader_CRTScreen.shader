Shader "Custom/Shader_CRTScreen"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_ScanTex("Scan Texture", 2D) = "white" {}
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_ScreenWidth("Screen Width", Float) = 512
		_ScreenHeight("Screen Height", Float) = 512
	}
	SubShader
	{
		Tags
        {
            "RenderPipeline"="UniversalPipeline"
        }
        LOD 100

		Pass
		{
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
				float4 _MainTex_ST;
				float4 _NoiseTex_ST;
                float _ScreenWidth;
                float _ScreenHeight;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_ScanTex);
            SAMPLER(sampler_ScanTex);
            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

			struct a2v
			{
				float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
			};

			v2f vert(a2v v)
			{
				v2f o;
                VertexPositionInputs posInputs = GetVertexPositionInputs(v.positionOS);
                o.positionCS = posInputs.positionCS;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
			}

			half4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv;

				float range = 0.02;
				float exp = 4;
				float a = range / pow((range - 0.5), exp);

				// corner twist
				float2 newUV;
				if (uv.x < 0.5 && uv.y < 0.5)
				{
					float ty = a*pow((uv.x-0.5), exp);
					float tx = a*pow((uv.y-0.5), exp);
					newUV = uv + float2(-tx*2*abs(uv.x-0.5), -ty*2*abs(uv.y-0.5));
				}
				else if (uv.x > 0.5 && uv.y < 0.5)
				{
					float ty = a*pow((uv.x-0.5), exp);
					float tx = -a*pow((uv.y-0.5), exp) + 1;
					newUV = uv + float2((1-tx)*2*abs(uv.x-0.5), -ty*2*abs(uv.y-0.5));
				}
				else if (uv.x < 0.5 && uv.y > 0.5)
				{
					float ty = -a*pow((uv.x-0.5), exp) + 1;
					float tx = a*pow((uv.y-0.5), exp);
					newUV = uv + float2(-tx*2*abs(uv.x-0.5), (1-ty)*2*abs(uv.y-0.5));
				}
				else
				{
					float ty = -a*pow((uv.x-0.5), exp) + 1;
					float tx = -a*pow((uv.y-0.5), exp) + 1;
					if (uv.y > ty || uv.x > tx)
					newUV = uv + float2((1-tx)*2*abs(uv.x-0.5), (1-ty)*2*abs(uv.y-0.5));
				}

				// noise offset
				float2 noiseUV = TRANSFORM_TEX(i.uv, _NoiseTex);
				newUV.x += 0.1 * SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, noiseUV).r;

				half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, newUV);
				col.rb *= step(0, sin(newUV.x * _ScreenWidth)+1) * 0.5 + 1;
				col.g *= (sin(newUV.x * _ScreenWidth)+1) * 0.5 + 1;
				
				// scan line
				half3 scanCol = half3(1, 1, 1);
				half4 pixelScanlineBrightness = half4(0.225, 0.85, 0.1, 0.95);
				float ssScanY = smoothstep(-1, 1, sin(newUV.x * _ScreenWidth));
				scanCol *= ssScanY * pixelScanlineBrightness.x + pixelScanlineBrightness.y;
				float ssScanX = smoothstep(-1, 1, sin((newUV.y + _Time.x)* _ScreenHeight));
				scanCol *= ssScanX * pixelScanlineBrightness.z + pixelScanlineBrightness.w;
				// scroll scan
				float2 scanUV = i.uv;
				scanUV.y += _Time.x;
				half4 scanLine = SAMPLE_TEXTURE2D(_ScanTex, sampler_ScanTex, scanUV);
				scanCol *= scanLine.rgb;
				float screenSvaerScanlineBrightness = 0.6;
				scanCol = scanCol * (1 - screenSvaerScanlineBrightness) + screenSvaerScanlineBrightness;
				col.rgb *= scanCol;

				// dark corner
				float dx = newUV.x;
				float dy = newUV.y;
				float ex = 0.2;
				if (newUV.x < 0.5)
					col.rgb *= pow(abs(dx)/0.5, ex);
				else
					col.rgb *= pow(abs(1-dx)/0.5, ex);
				if (newUV.y < 0.5)
					col.rgb *= pow(abs(dy)/0.5, ex);
				else
					col.rgb *= pow(abs(1-dy)/0.5, ex);
				
				// black corner
				if (newUV.x < 0 || newUV.x > 1 || newUV.y < 0 || newUV.y > 1)
					col.rgb *= 0;
				return col * 1.5;
			}

			ENDHLSL
		}
	}
}

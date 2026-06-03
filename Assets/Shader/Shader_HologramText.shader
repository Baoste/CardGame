Shader "Custom/HologramText"
{
    Properties
    {
        _MainTex ("Font Atlas", 2D) = "white" {}
        [HDR]_Color ("Color", Color) = (0.2, 1.5, 2.5, 1)
        _ScanSpeed ("Scan Speed", Float) = 3
        _ScanDensity ("Scan Density", Float) = 40
        _FlickerStrength ("Flicker", Float) = 0.05
        _AlphaClip ("Alpha Clip", Range(0,1)) = 0.5
        _EdgeSoftness ("Edge Softness", Range(0.001,0.2)) = 0.03
        _GlowStrength ("Glow Strength", Float) = 1.2
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha One
        ZWrite Off
        Cull Back

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct a2v
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _Color;
            float _ScanSpeed;
            float _ScanDensity;
            float _FlickerStrength;
            float _AlphaClip;
            float _EdgeSoftness;
            float _GlowStrength;

            v2f vert(a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float sdf = tex2D(_MainTex, i.uv).a;

                float alpha = smoothstep(
                    _AlphaClip - _EdgeSoftness,
                    _AlphaClip + _EdgeSoftness,
                    sdf
                );

                float scan = sin(_Time.y * _ScanSpeed + i.uv.y * _ScanDensity);
                scan = scan * 0.5 + 0.5;
                scan = pow(scan,3);

                float flicker = sin(_Time.y * 40.0) * _FlickerStrength + 1.0;

                alpha *= flicker * i.color.a;

                float3 color = _Color.rgb * i.color.rgb * _GlowStrength * scan;

                return float4(color, alpha);
            }
            ENDHLSL
        }
    }
}
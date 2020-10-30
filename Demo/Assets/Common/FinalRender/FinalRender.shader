Shader "Common/FinalRender"
{
    Properties
    {
        _Tex ("Texture", 2D) = "white" {}
        _FadeOut("FadeOut", Range(0.0,1.0)) = 0.0
        _Deform("Deform", Range(0.0,1.0)) = 0.0
        _DeformInvert("DeformInvert", Range(0.0,1.0)) = 0.0
        _DeformTex("DeformTex", 2D) = "white" {}
        _Pixelate("Pixelate", Range(0.0,160.0)) = 0.0
        _Scanline("Scanline", Range(0.0,160.0)) = 0.0
        _DeformSquare("DeformSquare", Range(0.0,1.0)) = 0.0
        _DeformSquareInvert("DeformSquareInvert", Range(0.0,1.0)) = 0.0
        _DeformSquareTex("DeformSquareTex", 2D) = "white" {}
        _Glitch("Glitch", 2D) = "white" {}
        _GlitchStrength("GlitchStrength", Range(0.0,1.0)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _Tex;
            float _FadeOut;
            float _Deform;
            float _DeformInvert;
            sampler2D _DeformTex;
            float4x4 _DeformRoll;
            float _Pixelate;
            float _Scanline;
            float _DeformSquare;
            float _DeformSquareInvert;
            sampler2D _DeformSquareTex;
            sampler2D _Glitch;
            float _GlitchStrength;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 glitch = tex2D(_Glitch, i.uv);
                glitch.xyz *= glitch.a*_GlitchStrength*0.5;
                glitch.a = 0.0;

                float4 d_square = tex2D(_DeformSquareTex, lerp(i.uv, float2(1.0, 1.0) - i.uv, _DeformSquareInvert)) * _DeformSquare*0.5;
                
                float2 duv = lerp(i.uv, float2(1.0, 1.0) - i.uv, _DeformInvert);
                float4 d = tex2D(_DeformTex, mul(_DeformRoll, float4(duv-0.5, 0, 1)).xy+0.5) * _Deform;
                d.a = 0.0;

                float s = pow(2.0, floor(_Pixelate*6.0)) * _ScreenParams.x / 1920.0;

                float2 uv = lerp(i.uv, floor((i.uv* _ScreenParams.xy) / s)* s / _ScreenParams.xy, _Pixelate);
                uv += d.xy * 0.015 + d_square.xy * 0.015;

                float4 t = tex2D(_Tex, uv) + d * 0.01 + d_square*0.008 + glitch;

                float bw = t.x * 0.21 + t.y * 0.72 + t.z * 0.07;
                float4 baw = float4(bw, bw, bw, 1.0);
                t = lerp(t, baw, fmod(i.uv.y * (_ScreenParams.y - 1.0), 1.0+ _Scanline *8.0));

                return lerp(t, 0.0, _FadeOut);
            }
            ENDCG
        }
    }
}

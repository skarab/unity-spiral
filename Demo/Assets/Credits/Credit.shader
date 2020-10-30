Shader "Demo/Credit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex("NoiseTex", 2D) = "white" {}
        _Fade("Fade", float) = 0.0
        _White("White", float) = 0.0
        _Scanline("Scanline", Range(0.0,160.0)) = 0.0
        _Height("Height", float) = 20.0
        _DeformSquare("DeformSquare", Range(0.0,1.0)) = 0.0
        _DeformSquareInvert("DeformSquareInvert", Range(0.0,1.0)) = 0.0
        _DeformSquareTex("DeformSquareTex", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

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

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float _Fade;
            float _White;
            float _Scanline;
            float _Height;
            float _DeformSquare;
            float _DeformSquareInvert;
            sampler2D _DeformSquareTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 d_square = tex2D(_DeformSquareTex, lerp(i.uv, float2(1.0, 1.0) - i.uv, _DeformSquareInvert)) * 0.5;

                float4 t = tex2D(_MainTex, i.uv+d_square.xy * 0.015)+ d_square * 0.008;
                float noise = tex2D(_NoiseTex, i.uv).x;
                t.a = t.a * step(noise, _Fade);
                t.r = _White;

                float bw = t.x * 0.21 + t.y * 0.72 + t.z * 0.07;
                float4 baw = float4(bw, bw, bw, t.a);
                t = lerp(t, baw, fmod(i.uv.y * (_Height - 1.0), 1.0 + _Scanline * 8.0));

                return t;
            }
            ENDCG
        }
    }
}

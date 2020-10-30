Shader "Demo/RenderShaderThing"
{
    Properties
    {
        _Cube("Reflection Map", Cube) = "" {}
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            uniform samplerCUBE _Cube;

            #define HASHSCALE4 float4(.1031, .1030, .0973, .1099)


            float4 hash41(float p)
            { // by Dave_Hoskins
                float4 p4 = frac(float4(p,p,p,p) * HASHSCALE4);
                p4 += dot(p4, p4.wzxy + 19.19);
                return frac((p4.xxyz + p4.yzzw) * p4.zywx);
            }

            // polynomial smooth min (k = 0.1);
            float smin(float a, float b, float k)
            { // by iq
                float h = clamp(0.5 + 0.5 * (b - a) / k, 0.0, 1.0);
                return lerp(b, a, h) - k * h * (1.0 - h);
            }

            float map(float3 p)
            {
                float s = length(p - float3(0.0, .01 * sin(8.0 * _Time.y), 0.0)) - 0.5;
                s += 0.005 * sin(45.0 * p.x + 10.0 * _Time.y);

                for (int i = 0; i < 12; ++i) {
                    float4 rnd = hash41(100.0 + float(i));
                    float3 rndPos = 2.0 * (normalize(rnd.xyz) - float3(0.5,0.5,0.5));
                    rndPos.y *= 0.2;
                    float timeOffset = rnd.w;
                    float phase = frac(timeOffset - 0.25 * _Time.y*4.0);
                    float3 offset = lerp(0.1 * rndPos, 15.0 * rndPos, phase);
                    float rnd2 = frac(rnd.x + rnd.y);
                    float s0 = length(p + offset) - 0.25 * lerp(0.8 + 0.2 * rnd2, 0.2 + 0.8 * rnd2, phase);
                    s = smin(s, s0, 0.4);
                }

                s += 0.002 * sin(20.0 * p.x + 10.0 * _Time.y);

                return s;
            }

            float3 env(float3 dir)
            {
                float3 cubemap = texCUBE(_Cube, dir).rgb;
                float ex = lerp(6.0, 12.0, 0.5 * (sin(0.5 * _Time.y) + 1.0));
                float t0 = 0.05 * pow(1.0 - dot(float3(0.0, -1.0, 0.0), dir), ex);
                float t1 = lerp(0.2, 2.5, 1.0 - abs(sin(2.0 * 3.14 * dir.y)));
                float3 c = cubemap * t0 * t1;
                return c * float3(0.35, 1.2, 2.5);
            }

            float3 calcNormal(float3 p)
            { // by iq
                float2 e = float2(1.0, -1.0) * 0.5773 * 0.0005;
                return normalize(e.xyy * map(p + e.xyy) +
                    e.yxy * map(p + e.yxy) +
                    e.yyx * map(p + e.yyx) +
                    e.xxx * map(p + e.xxx));
            }

            float3x3 setCamera(in float3 ro, in float3 ta, float cr)
            { // by iq
                float3 cw = normalize(ta - ro);
                float3 cp = float3(sin(cr), cos(cr), 0.0);
                float3 cu = normalize(cross(cw, cp));
                float3 cv = normalize(cross(cu, cw));
                return float3x3(cu, cv, cw);
            }

            float3 tonemapping(float3 color)
            { // by Zavie (lslGzl)
                color = max(float3(0,0,0), color - float3(0.004, 0.004, 0.004));
                color = (color * (6.2 * color + .5)) / (color * (6.2 * color + 1.7) + 0.06);
                return color;
            }

            float spline(float x, float x1, float x2, float y1, float dy1, float y2, float dy2)
            {
                float t = (x - x1) / (x2 - x1);
                float a = 2.0 * y1 - 2.0 * y2 + dy1 + dy2;
                float b = -3.0 * y1 + 3.0 * y2 - 2.0 * dy1 - dy2;
                float c = dy1;
                float d = y1;
                float t2 = t * t;
                float t3 = t2 * t;
                return a * t3 + b * t2 + c * t + d;
            }



            float4 frag (v2f i) : SV_Target
            {
                float2 u = i.uv;
                float2 v = 2.0 * (u - 0.5);
                v.x *= _ScreenParams.x / _ScreenParams.y;
                float2 m = float2(0,0);
                float cas = step(abs(v.y) * 2.39, _ScreenParams.x / _ScreenParams.y);
                if (cas < 0.1) return float4(0,0,0,0);

                float3 ro = float3(
                    8.0 * cos(0.2 * _Time.y + 6.0 * m.x),
                    0.2 * lerp(-1.0,1.0, m.y),
                    8.0 * sin(0.2 * _Time.y + 6.0 * m.x)
                );
                float taAnim = 2.0 * (smoothstep(-0.1, 0.1, sin(0.1 * _Time.y)) - 0.5);
                float3 ta = float3(taAnim, -0.05, 0.0);

                float3x3 ca = setCamera(ro, ta, 0.0);
                float3 rd = mul(ca, normalize(float3(v, lerp(4.5, 5.0, taAnim))));
                float3 p, c;
                float3 n, rl, rr;
                p = float3(0,0,0);
                c = env(rd);

                float t, d, a;
                t = d = a = 0.0;
                for (int i = 0; i < 50; ++i) {
                    t += (d = map(p = ro + rd * t));
                    if (d < 0.01) {
                        break;
                    }
                }

                float depth = length(p - ro);

                if (t < 25.0) { // if hit scene

                    n = calcNormal(p);
                    rl = reflect(rd, n);
                    rr = refract(rd, n, .19);

                    for (int i = 0; i < 25; ++i) {
                        d = map(p = ro + rd * t);
                        a += step(d, 0.008) * 0.005;
                        t += 0.02;
                    }

                    a = exp(-a * 25.0);
                    c = env(lerp(rr, rl, a));
                    c *= lerp(float3(1.4, 1.0, 0.9), float3(1,1,1), clamp(0.2 * length(p), 0.0, 1.0)); // value        
                    c *= lerp(float3(1.5,1.5,1.5), float3(.3,.3,.3), clamp(pow(0.1 * depth, 2.0), 0.0, 1.0)); // fogging
                    c *= 40.0;
                }

                //c = tonemapping(c);

                depth = 0.01 * dot(rd, p - ro); // depth paraller to camera
                return float4(c, depth);
            }
            ENDCG
        }
    }
}

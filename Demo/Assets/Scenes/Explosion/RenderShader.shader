Shader "Demo/RenderShader"
{
    Properties
    {
        _Noise("Texture", 2D) = "white" {}
        _MyTime("MyTime", float) = 0.0
        _CamPos("CamPos", Vector) = (0.0, 0.0, 0.0, 0.0)
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" "IgnoreProjector" = "True" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha

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

            sampler2D _Noise;
            float _MyTime;
            float4 _CamPos;
            float4x4 _CamDir;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            #define pi 3.14159265
            #define R(p, a) p=cos(a)*p+sin(a)*float2(p.y, -p.x)

            // iq's noise
            float noise(in float3 x)
            {
                float3 p = floor(x);
                float3 f = frac(x);
                f = f * f * (3.0 - 2.0 * f);
                float2 uv = (p.xy + float2(37.0, 17.0) * p.z) + f.xy;
                float2 rg = tex2Dlod(_Noise, float4((uv + 0.5) / 256.0, 0.0, 0.0)).yx;
                return 1. - 0.82 * lerp(rg.x, rg.y, f.z);
            }

            float fbm(float3 p)
            {
                return noise(p * .06125) * .5 + noise(p * .125) * .25 + noise(p * .25) * .125 + noise(p * .4) * .2;
            }

            float Sphere(float3 p, float r)
            {
                float3 q = abs(p) - float3(1,1,1)*0.2;
                return length(max(q, 0.0)) + min(max(q.x, max(q.y, q.z)), 0.0) - r;
            }

            const float nudge = 4.;	// size of perpendicular vector
            float SpiralNoiseC(float3 p)
            {
                float normalizer = 1.0 / sqrt(1.0 + nudge * nudge);	// pythagorean theorem on that perpendicular to maintain scale
                float n = 2.1-fmod(_MyTime * 0.2, -4.0); // noise amount
                float iter = 2.0;
                for (int i = 0; i < 8; i++)
                {
                    // add sin and cos scaled inverse with the frequency
                    n += -abs(sin(p.y * iter) + cos(p.x * iter)) / iter;	// abs for a ridged look
                    // rotate by adding perpendicular and scaling down
                    p.xy += float2(p.y, -p.x) * nudge;
                    p.xy *= normalizer;
                    // rotate on other axis
                    p.xz += float2(p.z, -p.x) * nudge;
                    p.xz *= normalizer;
                    // increase the frequency
                    iter *= 1.733733;
                }
                return n;
            }

            float VolumetricExplosion(float3 p)
            {
                float final = Sphere(p, 1.);
                final += fbm(p * 500.);
                final += SpiralNoiseC(p.xyz * 0.4132 + p.zxy * 0.4132 + 333.) * 1.25;

                return final;
            }

            float map(float3 p)
            {
                R(p.xz, _MyTime * 0.1);

                float VolExplosion = VolumetricExplosion(p / 0.5) * 0.5; // scale

                return VolExplosion;
            }

            float3 computeColor(float density, float radius)
            {
                // color based on density alone, gives impression of occlusion within
                // the media
                float3 result = lerp(float3(1.0, 0.9, 0.8), float3(0.4, 0.15, 0.1), density);

                // color added to the media
                float3 colCenter = 7. * float3(1.0, 0.0, 0.8);
                float3 colEdge = 1.5 * float3(0.5, 0.53, 0.48);
                result *= lerp(colCenter, colEdge, min((radius + .05) / .9, 1.15))*0.3;

                return result;
            }

            bool RaySphereIntersect(float3 org, float3 dir, out float near, out float far)
            {
                float b = dot(dir, org);
                float c = dot(org, org) - 8.;
                float delta = b * b - c;
                if (delta < 0.0)
                    return false;
                float deltasqrt = sqrt(delta);
                near = -b - deltasqrt;
                far = -b + deltasqrt;
                return far > 0.0;
            }

            float4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // ro: ray origin
                // rd: direction of the ray
                float3 rd = normalize(float3((i.uv - 0.5)/float2(_ScreenParams.y, _ScreenParams.x)*_ScreenParams.y*5.0, 1.));
                rd = mul(_CamDir, float4(rd, 1));
                rd.x = -rd.x;
                float3 ro = _CamPos; // float3(0., 0., _CamPos.z * 2.0);
                ro.x = -ro.x;

                // ld, td: local, total density 
                // w: weighting factor
                float ld = 0., td = 0., w = 0.;

                // t: length of the ray
                // d: distance function
                float d = 1., t = 0.;

                const float h = 0.1;

                float4 sum = float4(0,0,0,0);

                float min_dist = 0.0, max_dist = 0.0;

                float a = 0.0;

                if (RaySphereIntersect(ro, rd, min_dist, max_dist))
                {
                    
                t = min_dist * step(t,min_dist);

                // raymarch loop
                for (int i = 0; i < 86; i++)
                {

                    float3 pos = ro + t * rd;

                    // Loop break conditions.
                    if (td > 0.9 || d < 0.12 * t || t>10. || sum.a > 0.99 || t > max_dist) break;

                    // evaluate distance function
                    float d = map(pos);

                    
                    // change this string to control density 
                    d = max(d,0.001);

                    // point light calculations
                    float3 ldst = float3(0,0,0) - pos;
                    float lDist = max(length(ldst), 0.001);

                    // the color of light 
                    float3 lightColor = float3(10,0.5,0.25);

                    sum.rgb += (lightColor / (lDist*100.0));

                    if (d < h)
                    {
                        // compute local density 
                        ld = h - d;

                        // compute weighting factor 
                        w = (1. - td) * ld;

                        // accumulate density
                        td += w + 1. / 200.;

                        float4 col = float4(computeColor(td,lDist), td);

                        // emission
                        sum += sum.a * float4(sum.rgb, 0.0) * 0.2 / lDist;

                        // uniform scale density
                        col.a *= 0.2;
                        // colour by alpha
                        col.rgb *= col.a;
                        // alpha blend in contribution
                        sum = sum + col * (1.0 - sum.a);

                        a = ld*10.0+0.2;
                    }

                    td += 1. / 70.;

                    // trying to optimize step size
                    t += max(d * 0.08 * max(min(length(ldst),d),2.0), 0.01);
                }

                // simple scattering
                sum *= 1. / exp(ld * 0.2) * 0.8;
                
                sum = clamp(sum, 0.0, 1.0);

                sum.xyz = sum.xyz * sum.xyz * (3.0 - 2.0 * sum.xyz);

                }

                //a *= -fmod(_MyTime * 0.2, -4.0) * 0.25;
                return float4(sum.xyz,a);
            }
            ENDCG
        }
    }
}

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Glitch"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseVal ("Noise Value",Range(0.0,1.0)) = 0.0
        _Direction ("Effect Direction",Float) = 1.0
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        Fog { Mode Off }

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
                float2 uv_noise : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.uv_noise = v.uv;
                return o;
            }
            
            sampler2D _MainTex;
            float _NoiseVal;
            float _Direction;
            float _ExplodeVal;

			float3 mod289(float3 x) {
				return x - floor(x * (1.0 / 289.0)) * 289.0;
			}

			float2 mod289(float2 x) {
				return x - floor(x * (1.0 / 289.0)) * 289.0;
			}

			float3 permute(float3 x) {
				return mod289(((x*34.0)+1.0)*x);
			}

			float snoise(float2 v){
			  const float4 C = float4(0.211324865405187,  // (3.0-sqrt(3.0))/6.0
			                      0.366025403784439,  // 0.5*(sqrt(3.0)-1.0)
			                     -0.577350269189626,  // -1.0 + 2.0 * C.x
			                      0.024390243902439); // 1.0 / 41.0
			// First corner
			  float2 i  = floor(v + dot(v, C.yy) );
			  float2 x0 = v -   i + dot(i, C.xx);

			// Other corners
			  float2 i1;
			  //i1.x = step( x0.y, x0.x ); // x0.x > x0.y ? 1.0 : 0.0
			  //i1.y = 1.0 - i1.x;
			  i1 = (x0.x > x0.y) ? float2(1.0, 0.0) : float2(0.0, 1.0);
			  // x0 = x0 - 0.0 + 0.0 * C.xx ;
			  // x1 = x0 - i1 + 1.0 * C.xx ;
			  // x2 = x0 - 1.0 + 2.0 * C.xx ;
			  float4 x12 = x0.xyxy + C.xxzz;
			  x12.xy -= i1;

			// Permutations
			  i = mod289(i); // Avoid truncation effects in permutation
			  float3 p = permute( permute( i.y + float3(0.0, i1.y, 1.0 ))
					+ i.x + float3(0.0, i1.x, 1.0 ));

			  float3 m = max(0.5 - float3(dot(x0,x0), dot(x12.xy,x12.xy), dot(x12.zw,x12.zw)), 0.0);
			  m = m*m ;
			  m = m*m ;

			// Gradients: 41 points uniformly over a line, mapped onto a diamond.
			// The ring size 17*17 = 289 is close to a multiple of 41 (41*7 = 287)

			  float3 x = 2.0 * frac(p * C.www) - 1.0;
			  float3 h = abs(x) - 0.5;
			  float3 ox = floor(x + 0.5);
			  float3 a0 = x - ox;

			// Normalise gradients implicitly by scaling m
			// Approximation of: m *= inversesqrt( a0*a0 + h*h );
			  m *= 1.79284291400159 - 0.85373472095314 * ( a0*a0 + h*h );

			// Compute final noise value at P
			  float3 g;
			  g.x  = a0.x  * x0.x  + h.x  * x0.y;
			  g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			  return 130.0 * dot(m, g);
			}

			float rand(float2 co) {
			   return frac(sin(dot(co.xy,float2(12.9898,78.233))) * 43758.5453);
			}

            fixed4 frag (v2f i) : SV_Target
            {
                //fixed4 col = tex2D(_MainTex, i.uv);
                float time = 0.1;

				// Create large, incidental noise waves
			    float noise = max(0.0, snoise(float2(time, i.uv.y * 0.3)) - 0.3) * (1.0 / 0.7);
			    
			    // Offset by smaller, constant noise waves
			    noise = noise + (snoise(float2(time*10.0, i.uv.y * 2.4)) - 0.5) * 0.15;
			    
			    // Apply the noise as x displacement for every line
			    noise = noise*_NoiseVal;
			    float xpos = i.uv.x - noise * noise * 0.25;
			    if(_Direction == -1.0){
			    	xpos = i.uv.x + noise * noise * 0.25;
			    }
			    fixed4 col = tex2D(_MainTex, float2(xpos, i.uv.y));

			    // Mix in some random interference for lines
			    //float rval = rand(float2(i.uv.y * time,i.uv.y * time));
			    //col.rgb = lerp(col.rgb, float3(rval,rval,rval), noise * 0.3).rgb;
			    
			    // Apply a line pattern every 4 pixels

			    if (floor(fmod(i.uv.y * 0.25, 2.0)) == 0.0)
			    {
			        col.rgb *= 1.0 - (0.15 * noise);
			    }


			    // Shift green/blue channels (using the red channel)

			    col.g = lerp(col.g, tex2D(_MainTex, float2(xpos + noise * 0.05, i.uv.y)).g, 0.25);
			    col.b = lerp(col.b, tex2D(_MainTex, float2(xpos - noise * 0.05, i.uv.y)).b, 0.25);


                return col;
            }
            ENDCG
        }
    }
}
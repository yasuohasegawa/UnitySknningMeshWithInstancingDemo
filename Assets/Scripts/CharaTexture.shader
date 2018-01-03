Shader "Unlit/CharaTexture"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 100

		ZWrite Off
		Cull off
		Blend SrcAlpha OneMinusSrcAlpha 

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			#define MAX_VERTS 12

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float2 uv2 : TEXCOORD1;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _zval;

			float nrand(float2 uv) {
			    return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
			}

			v2f vert (appdata v)
			{
				v2f o;

				int vid = v.uv2.x;
				float rnd = nrand(v.uv);

				v.vertex.y += sin(_Time.w*(fmod(floor(vid/MAX_VERTS), 10.0)+2.0)*(0.2+(rnd*0.001)))*0.1+(rnd*0.1);
				v.vertex.x += sin((fmod(floor(vid/MAX_VERTS), 10.0)+2.0)*_zval)*0.5;

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog
				//UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}

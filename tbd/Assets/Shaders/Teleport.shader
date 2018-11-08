Shader "Unlit/Teleport"
{
	Properties
	{
		_MainTex("Vignette", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
		

			float random(float2 co)
			{
				float ret = sin(dot(co.xy ,float2(12.9898,78.233)));
				ret = (ret * 43758.5453 + 1.0f) / 2.0f;
				return ret;
			}

			float2 random(float2 min, float2 max, float2 seed)
			{
				float2 ret;
				ret.x = min.x + random(seed) * (max.x - min.x);
				ret.y = min.y + random(seed) * (max.y - min.y);
				return ret;
			}

			float distance(float2 a, float2 b)
			{
				float2 d = a - b;
				return sqrt(d.x * d.x + d.y * d.y);
			}
			
			// Ok now I actually know what I am doing. woo
			fixed4 frag (v2f i) : SV_Target
			{
				float frequency = 3.0f;
				float2 st = i.uv * frequency;
				int2 st_i = (int2)st;
		
				float min_dist = 100000.0f;
				for(int x = -1; x <= 1; ++x)
				{
					for(int y = -1; y <= 1; ++y)
					{
						int2 n_d = int2(x, y);
						float2 n_min = float2(st_i + n_d);
						float2 n_max = n_min + float2(1.0f, 1.0f);
						float2 n = random(n_min, n_max, n_max);
						float dist = distance(st, n);
						if(dist < min_dist)
						{
							min_dist = dist;
						}
					}
				}

				//fixed4 col = tex2D(_MainTex, i.uv);
				//col = col * col * col * col;
				fixed4 col = fixed4(0.0f, 0.0f, 0.0f, 0.0f);
				col += min_dist * 0.5;
				return col;
			}
			ENDCG
		}
	}
}

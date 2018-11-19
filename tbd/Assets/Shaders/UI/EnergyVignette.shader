Shader "Unlit/EnergyVignette"
{
	Properties
	{
		_MainTex("Vignette", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		_VignetteMinMax("Vignette", Vector) = (0.8, 1.0, 0.0, 0.0)
		_UnscaledTime("UnscaledTime", Float) =  0.0
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
			float4 _MainTex_ST;
			float4 _Color;
			float2 _VignetteMinMax;
			float _UnscaledTime;

			#include "../Utility.shader"
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float f = 10.0;
				float2 st = i.uv * f;
				float2 st_i = floor(st);
				float2 st_f = frac(st);
		
				float min_dist = 100000.0;
				for(int x = -1; x <= 1; ++x)
				{
					for(int y = -1; y <= 1; ++y)
					{
						int2 n_d = int2(x, y);
						float2 n_min = st_i + (float2)n_d;
						float2 n_f = random2(n_min);

						float2 min = float2(1.0, 1.0);
						float2 delta = float2(1.0, 1.0);
						n_f = (sin(_UnscaledTime * lerp_delta(min, delta, n_f)) + 1.0) / 2.0;

						float2 n = n_f + n_min;
						float2 diff = st - n;
						float dist = length(diff);
						if(dist < min_dist)
						{
							min_dist = dist;
						}
					}
				}
				fixed4 color = fixed4(0.02, 0.8, 1.0, 1.0);

				fixed4 final_color = fixed4(0.0, 0.0, 0.0, 0.0);
				final_color += min_dist;
				float vign_min = _VignetteMinMax.x;
				float vign_max = _VignetteMinMax.y;
				float vign = vignette(i.uv, vign_min, vign_max);
				final_color *= vign;
				final_color *= _Color;
				return final_color;
			}
			ENDCG
		}
	}
}

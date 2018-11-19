Shader "Unlit/RadialBar"
{

	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_InsideColor("InsideColor", Color) = (1.0, 1.0, 1.0, 1.0)
		_OutsideColor("OutsideColor", Color) = (1.0, 1.0, 1.0, 1.0)
		_InsideOutsideDistance("InsideOutsideDistance", Float) = 0.0
		_BarMinMax("BarMinMax", Vector) = (0.0, 0.0, 0.0, 0.0)
		_RadianMinMax("RadianMinMax", Vector) = (0.0, 0.0, 0.0, 0.0)
		_NoiseFrequency("NoiseFrequency", Float) = 1.0
		_NoiseContribution("NoiseContribution", Float) = 1.0
		_NoiseSpeed("NoiseSpeed", Float) = 1.0
		_Percentage("Percentage", Float) = 0.25
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
			float4 _InsideColor;
			float4 _OutsideColor;
			float _InsideOutsideDistance;
			float4 _RadianMinMax;
			float4 _BarMinMax;
			float _NoiseFrequency;
			float _NoiseContribution;
			float _NoiseSpeed;
			float _Percentage;
			
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
				fixed4 color = fixed4(0.0, 0.0, 0.0, 0.0);

				float2 uv_centered = (i.uv - 0.5) * 2.0;
				float2 st = (i.uv) * _NoiseFrequency;
				float2 time = _Time * _NoiseSpeed;	
				time.x = 0.0;
				float noise_height = noise(st - time) * _NoiseContribution;
				float len = length(uv_centered);
				float2 unit = unit_vector(uv_centered);
				float rad = acos(unit.x);
				if(unit.y < 0.0)
				{
					rad = rad * -1.0 + TAU;
				}
				float rad_min = _RadianMinMax.x;
				float rad_max = _RadianMinMax.y;
				rad_max = rad_min + _Percentage * (rad_max - rad_min);
				bool fill = len >= _BarMinMax.x && 
							len <= _BarMinMax.y &&
							rad >= rad_min &&
							rad <= rad_max;
				if(fill)
				{
					float comparison[8];
					comparison[0] = _BarMinMax.x;
					comparison[1] = len;
					comparison[2] = _BarMinMax.y;
					comparison[3] = len;
					comparison[4] = rad_min;
					comparison[5] = rad;
					comparison[6] = rad_max;
					comparison[7] = rad;
					float min_dist = _InsideOutsideDistance;
					for(int i = 0; i < 8; i += 2)
					{
						float dist = comparison[i] - comparison[i + 1];
						dist = abs(dist);
						if(dist < min_dist)
						{
							min_dist = dist;
						}
					}
					if(min_dist > _InsideOutsideDistance)
					{
						color = _InsideColor;
					}
					else
					{
						float lerp_param = min_dist / _InsideOutsideDistance;
						color = _OutsideColor + lerp_param * (_InsideColor - _OutsideColor);
						color = color + noise_height * (_OutsideColor - color);
					}
				}
				return color;
			}
			ENDCG
		}
	}
}

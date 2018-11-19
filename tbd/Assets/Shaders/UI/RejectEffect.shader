Shader "Unlit/RejectEffect"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		_VignetteMinMax("Vignette", Vector) = (0.8, 1.0, 0.0, 0.0)
		_Percentage("Percentage", Float) = 0.0
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
				float2 uv_centered = (i.uv * 2.0) - 1.0;
				float begin = _VignetteMinMax.x;
				float end = sqrt(2.0);

				float span = end - begin;
				float center_dist = (length(uv_centered) - begin) / end;
				float start = PI * (center_dist) + PI;
				float exists = (sign(sin(start - _Percentage * TAU)) + 1.0) / 2.0;
				exists *= (sign(sin(start * 20.0 * TAU)) + 1.0) / 2.0;



				//exists = dist_from_origin / sqrt(2);

				float vign = vignette(i.uv, _VignetteMinMax.x, _VignetteMinMax.y);
				fixed4 color = vign * _Color * exists;
				return color;

			}
			ENDCG
		}
	}
}
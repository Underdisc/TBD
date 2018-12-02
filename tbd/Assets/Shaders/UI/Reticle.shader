Shader "Unlit/Reticle"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_ContainMax("Contain Max", Range(0.0, 1.0)) = 0.25
		_ContainWidth("Contain Width", Range(0.0, 1.0)) = 0.75
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
			float _ContainMax;
			float _ContainWidth;
			
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
				float aspect = _ScreenParams.x / _ScreenParams.y;
				float2 center_delta = ((i.uv * 2.0) - float2(1.0, 1.0));
				center_delta.x = center_delta.x * aspect;
				float mag = length(center_delta);
				float contain_min = _ContainMax - _ContainWidth;
				float exists = gteq(mag, contain_min) * lteq(mag, _ContainMax);
				fixed4 color = _Color;
				color.w = exists;
				return color;
			}
			ENDCG
		}
	}
}

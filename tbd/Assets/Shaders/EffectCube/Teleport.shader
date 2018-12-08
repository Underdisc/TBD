Shader "Unlit/Teleport"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Forward("Forward", Vector) = (1.0, 0.0, 0.0, 0.0)
		_ContainMinMax("Contain Min Max", Vector) = (0.25, 0.75, 0.0, 0.0)
		_ContainMin("Contain: Min", Range(0.0, 1.0)) = 0.25
		_ContainMax("Contain: Max", Range(0.0, 1.0)) = 0.75
		_CombFrequency("Comb Frequency", Float) = 1.0
		_CombDepth("Comb Depth", Range(0.0, 1.0)) = 0.05
		_CombColor("Comb Color", Color) = (0.0, 0.0, 1.0, 0.0)
		_NoiseFrequencyForward("Noise Frequency: Forward", Float) = 1.0
		_NoiseFrequencyRight("Noise Frequency: Right", Float) = 1.0
		_NoiseSpeed("Noise Speed", Float) = 1.0
		_ContainGradientMin("Contain Gradient: Min", Range(0.0, 1.0)) = 0.25
		_ContainGradientMax("Contain Gradient: Max", Range(0.0, 1.0)) = 0.75
		_ToonLevel("Toon Level", Int) = 4
		_DissolvePercentage("Dissolve Percentage", Range(0.0, 1.0)) = 0.5
		_DissolveFrequencyForward("Dissolve Frequency: Forward", Float) = 1.0
		_DissolveFrequencyRight("Dissolve Frequency: Right", Float) = 1.0
		_CurrentTime("Current Time", Float) = 0.0
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Opaque" }
		LOD 100

		Blend SrcAlpha OneMinusSrcAlpha


		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "../Utility.shader"

			struct v2f
			{
				half3 modelPosition : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Forward;
			float _ContainMin;
			float _ContainMax;
			float _CombFrequency;
			float _CombDepth;
			float4 _CombColor;
			float _NoiseFrequencyForward;
			float _NoiseFrequencyRight;
			float _NoiseSpeed;
			float _ContainGradientMin;
			float _ContainGradientMax;
			float _ToonLevel;
			float _DissolvePercentage;
			float _DissolveFrequencyForward;
			float _DissolveFrequencyRight;
			float _CurrentTime;


			v2f vert (float4 vertex : POSITION)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(vertex);
				o.modelPosition = vertex;
				return o;
			}

			// You need to make the right side continuous. Just look at the right side
			// of the effect and you will see it. 
			fixed4 frag (v2f i) : SV_Target
			{
				float3 global_up = float3(0.0, 1.0, 0.0);

				float3 loc =  i.modelPosition.xyz;
				// The fragment direction.
				float3 fdir = normalize(loc);
				float3 forward = normalize(_Forward.xyz);

				float dir_dot_fdir = dot(fdir, forward);
				float theta_forward = acos(dir_dot_fdir);
				float perc_forward = theta_forward / PI;


				float3 right = cross(forward, global_up);
				// The frag dir projected onto a plane where the plane normal is
				// the forward vector.
				float3 fdirproj = project_onto_plane(fdir, forward);
				// A normalized version of the fragment direction projected onto the
				// forward plane.
				float3 fdirproj_n = normalize(fdirproj);
				float fdirproj_dot_right = dot(fdirproj_n, right);
				// Because frdirproj_dot_right might be less than -1 or greater than 1
				// we need to restrict fdirproj_dot_right to a [-1, 1] range so all
				// values given to acos give defined outputs.
				fdirproj_dot_right = clamp(fdirproj_dot_right, -1.0, 1.0);
				float theta_right = acos(fdirproj_dot_right);
				float perc_right = theta_right / PI;
				float c_min = abs(sin(perc_right * PI * _CombFrequency) * _CombDepth);
				c_min += _ContainMin;
				float c_max = _ContainMax;

				float3 down = cross(forward, right);
				// If the fragment direction is outside of the down half space, this will 
				// be one, otherwise, it will be 0.
				float fdir_out_down_space = gt(dot(fdir, down), 0.0);
				// Theta right full is a value between 0 and 2PI. It is angle between
				// the fragment direction and the right vector, but the bottom side of
				// the circle [PI, 2PI] is accounted for.
				float theta_right_full = abs(theta_right - fdir_out_down_space * PI);
				theta_right_full += fdir_out_down_space * PI;
				float perc_right_full = theta_right_full / TAU;

				float noise_perc_forward = perc_forward - _CurrentTime * _NoiseSpeed;
				noise_perc_forward *= _NoiseFrequencyForward;
				float noise_perc_right = perc_right_full * _NoiseFrequencyRight;
				float2 noise_perc_vec = float2(noise_perc_forward, noise_perc_right);
				float perc_noise = noise(noise_perc_vec);

				float g_min = _ContainGradientMin;
				float g_max = _ContainGradientMax;

				float gradient_forward = gradient(g_min, g_max, perc_forward);
				float grayscale = apply_gradient(gradient_forward, perc_noise);
				grayscale = toon(grayscale, _ToonLevel);
				grayscale = invert(grayscale, 0.0, 1.0);
				float4 color = grayscale * _CombColor;

				noise_perc_forward = perc_forward * _DissolveFrequencyForward;
				noise_perc_right = perc_right_full * _DissolveFrequencyRight;
				noise_perc_vec = float2(noise_perc_forward, noise_perc_right);
				perc_noise = noise(noise_perc_vec);
				perc_noise = clamp(pow(2.0, perc_noise) - 1.0, 0.0, 1.0);
				float dissolve = lt(_DissolvePercentage, perc_noise);

				// Store the transparency value of this fragment.
				float transparency = gt(perc_forward, c_min);
				transparency *= lt(perc_forward, c_max);
				transparency *= dissolve;
				fixed4 final_fragment = float4(color.xyz, transparency);
				return final_fragment;
			}
			ENDCG
		}
	}
}

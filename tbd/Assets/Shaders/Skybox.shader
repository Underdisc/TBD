Shader "Skybox/Bars"
{
    // no Properties block this time!
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _BarFrequency("Bar Frequency", Float) = 10.0
        _NoiseFrequency("Noise Frequency", Float) = 10.0
    }

    // A few rules about unity skyboxes.
    // The box used for the skybox is a 2 x 2 x 2.
    //
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // include file that contains UnityObjectToWorldNormal helper function
            #include "UnityCG.cginc"    
            #include "UnityShaderVariables.cginc"
            #include "Utility.Shader"

            float4 _Color;
            float _BarFrequency;
            float _NoiseFrequency;

            struct v2f {
                // we'll output world space normal as one of regular ("texcoord") interpolators
                half3 modelPosition : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            // vertex shader: takes object space normal as input too
            v2f vert (float4 vertex : POSITION, float3 normal : NORMAL)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(vertex);
                // UnityCG.cginc file contains function to transform
                // normal from object to world space, use that
                o.modelPosition = vertex;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c = 0;
                // normal is a 3D vector with xyz components; in -1..1
                // range. To display it as color, bring the range into 0..1
                // and put into red, green, blue components

                const float3 d_vec = float3(1.0, 0.0, 0.0);
                float3 direction = float3(i.modelPosition.xyz);
                direction = normalize(direction);

                float angle = acos(dot(direction, d_vec));
                float w = angle * 2.0 * _BarFrequency;
                float alive = gt(sin(w), 0.0);

                float noise_val = noise(direction.xy * _NoiseFrequency);
                alive = alive * gt(noise_val, 0.5);
                
                
                c = _Color * alive;
                return c;
            }
            ENDCG
        }
    }
}
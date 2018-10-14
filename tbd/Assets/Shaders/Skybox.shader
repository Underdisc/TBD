Shader "Unlit/Skybox"
{
    // no Properties block this time!
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
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

            float4 _Color;

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
                
                float3 direction = i.modelPosition;
                direction = normalize(direction);

                

                float voltage = sin(direction.y * 20 + _Time * 100);
                float3 color = voltage * _Color.rgb;
                c.rgb = color;
                return c;
            }
            ENDCG
        }
    }
}
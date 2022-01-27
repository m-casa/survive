Shader "Hidden/ScreenDepthNormal"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;

            // Get depth and normals
            sampler2D _CameraDepthNormalsTexture;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float4 normalDepth; // Assign normalXYZ to floatXYZ, depth to floatW

                // Decode depth normal maps: (InputTex, out depth, out normalXYZ)
                DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, i.uv), normalDepth.w, normalDepth.xyz);

                // Test depth pass
                col.rgb = normalDepth.w;

                // Test normal pass
                //col.rgb = normalDepth.xyz;

                // Just invert the colors
                //col.rgb = 1 - col.rgb;

                // Test green coloring
                //col.r = 0; col.g = 1;

                return col;
            }
            ENDCG
        }
    }
}

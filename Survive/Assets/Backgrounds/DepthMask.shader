Shader "Custom/DepthMask"
{
    SubShader
    {
        Tags { "Queue" = "Geometry-1" }
        Lighting On
        Pass
        {
            ZWrite On
            ZTest LEqual
            ColorMask 0
        }
    }
}

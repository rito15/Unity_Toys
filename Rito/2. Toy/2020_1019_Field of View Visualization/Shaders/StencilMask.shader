
Shader "Custom/StencilMask"
{
    Properties
    {

    }
    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="Geometry-100" }
        //Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        //ColorMask 0
        ZWrite off

        Stencil
        {
            Ref 1
            Comp Always
            Pass replace
        }

        CGPROGRAM

        #pragma surface surf nolight noambient noforwardadd nolightmap novertexlights noshadow alpha:fade
        //#pragma surface surf nolight alpha:fade
        #pragma target 3.0

        struct Input
        {
            float4 color:COLOR;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            o.Alpha = 0;
            o.Albedo = float3(0, 0, 0);
        }

        half4 Lightingnolight(SurfaceOutput s, half3 lightDir, half atten)
        {
            return float4(0, 0, 0, 0);
        }

        ENDCG
    }
    FallBack "Diffuse"
}

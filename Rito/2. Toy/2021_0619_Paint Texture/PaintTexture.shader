// https://www.patreon.com/posts/rendertexture-15961186
// https://pastebin.com/LxDYqWBh

Shader "Rito/PaintTexture"
{
    Properties
    {
        _Color ("Tint Color", Color) = (1, 1, 1, 1)
        _MainTex ("Main Texture", 2D) = "white" {}
        _PaintTex ("Painted Texture", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        sampler2D _PaintTex;
        fixed4 _Color;

        struct Input
        {
            float2 uv_MainTex;
        };

        UNITY_INSTANCING_BUFFER_START(Props)

            //UNITY_DEFINE_INSTANCED_PROP(sampler2D, _PaintTex)

        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            //sampler2D paintTex = UNITY_ACCESS_INSTANCED_PROP(Props, _PaintTex);

            fixed4 main = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            fixed4 painted = tex2D (_PaintTex, IN.uv_MainTex);

            o.Emission = lerp(main.rgb, painted.rgb, painted.a);

            o.Alpha = main.a * painted.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}

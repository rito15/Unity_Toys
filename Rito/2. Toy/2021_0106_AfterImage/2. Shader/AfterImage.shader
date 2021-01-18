// 설명 : 
Shader "Unlit/AfterImage"
{
    Properties
    {
        [HDR]_ColorA("ColorA", Color) = (1.0, 0.0, 0.0, 1.0)
        [HDR]_ColorB("ColorB", Color) = (0.0, 1.0, 0.0, 1.0)
        [HDR]_ColorC("ColorC", Color) = (0.0, 0.0, 1.0, 1.0)
        _Alpha("Alpha", Range(0, 1)) = 1
        _Intensity("Intensity", Range(0, 2)) = 0.8
        _FresnelPower("FresnelPower", Range(0, 10)) = 8
        [Toggle(FRESNEL_INVERT)] _FresnelInvert("Fresnel Invert", Float) = 0
        [HideInInspector] _Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        [HideInInspector]_ColorFactor("Color Factor", Float) = 0
        [HideInInspector]_MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma shader_feature FRESNEL_INVERT
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                half3 worldNormal : TEXCOORD1;
                float3 worldViewDir : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _ColorA;
            float4 _ColorB;
            float4 _ColorC;
            float _ColorFactor;
            float _Alpha;
            float _FresnelPower;
            float _Intensity;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                //o.worldViewDir = normalize(UnityWorldSpaceViewDir(v.vertex));
                o.worldViewDir = normalize(ObjSpaceViewDir(v.vertex));

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //float fresnel = dot(i.worldNormal, i.worldViewDir);
                float fresnel = max(dot(i.worldNormal, i.worldViewDir), dot(i.worldNormal, -i.worldViewDir));

#ifdef FRESNEL_INVERT
                fresnel = saturate(fresnel);
#else
                fresnel = 1.0 - saturate(fresnel);
#endif

                fresnel = pow(fresnel, _FresnelPower);

                //_Color = lerp(_ColorA, _ColorB, _ColorFactor);
                // 3컬러 혼함
                float stepAB = step(_ColorFactor, 0.5);
                float stepBC = 1 - stepAB;
                float abT = (_ColorFactor * 2.0) * stepAB;
                float bcT = ((_ColorFactor - 0.5) * 2.0) * stepBC;
                _Color = lerp(_ColorA, _ColorB, abT) * stepAB
                       + lerp(_ColorB, _ColorC, bcT) * stepBC;

                _Color.a = _Alpha * fresnel;
                _Color.rgb += _Intensity;

                return _Color;

                //float4 tc = float4(sin(_Time.y * 4.0), cos(_Time.y * 2.0), cos(_Time.y * 3.0), 0.0);
                //return c * (_Color) + tc;
            }
            ENDCG
        }
    }
}

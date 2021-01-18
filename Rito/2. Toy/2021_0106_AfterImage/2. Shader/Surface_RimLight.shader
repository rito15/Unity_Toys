Shader "Custom/RimLight" 
{
	Properties
	{
		[HDR]_Color1("Color", Color) = (1,0,0,1)
		[HDR]_Color2("Color2", Color) = (0,0,1,1)
		//[HideInInspector]_MainTex("Albedo (RGB)", 2D) = "white" {}
		[HideInInspector]_ColorFactor("Color Factor", float) = 1
		[HideInInspector]_Alpha("Alpha", float) = 1
		_RimLightMul("RimLightMul", Range(0, 10)) = 0.5
		_RimLightPow("RimLightPow", Range(0, 10)) = 1.5
		_Intensity("Intensity", Range(0, 10)) = 1
	}
	SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent"}
		LOD 200

		CGPROGRAM

		#pragma surface surf NoLighting alpha:fade noshadow  noambient novertexlights nolightmap nodynlightmap nodirlightmap nofog nometa noforwardadd nolppv noshadowmask interpolateview

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 2.0

		//sampler2D _MainTex;

		struct Input {
			//fixed2 uv_MainTex;
			fixed3 viewDir;
		};

		fixed4 _Color;
		fixed4 _Color1;
		fixed4 _Color2;
		fixed _RimLightMul;
		fixed _RimLightPow;

		half _ColorFactor;
		half _Alpha;
		float _Intensity;

		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutput o) {

			fixed NdotV = dot(IN.viewDir, o.Normal);
			fixed3 RimLight = saturate(pow(((1 - NdotV) * _RimLightMul), _RimLightPow));

			o.Albedo = lerp(_Color1, _Color2, _ColorFactor);
			//o.Alpha = saturate(RimLight * _Color1.a);
			o.Alpha = saturate(_Alpha * RimLight);
		}

		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			fixed4 c;
			c.rgb = s.Albedo + _Intensity;
			c.a = s.Alpha;// *(1 - _ColorFactor);
			return c;
		}

		ENDCG

	}
	FallBack ""
}

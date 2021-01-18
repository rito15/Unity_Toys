Shader "Custom/AfterImage"
{
	Properties
	{
		[HideInInspector] _Color("Color", Color) = (1,0,0,1)
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


		struct Input {
		fixed3 viewDir;
	};

	fixed4 _Color;
	fixed _RimLightMul;
	fixed _RimLightPow;

	half _Alpha;
	float _Intensity;


	void surf(Input IN, inout SurfaceOutput o) {

		fixed NdotV = dot(IN.viewDir, o.Normal);
		fixed3 RimLight = saturate(pow(((1 - NdotV) * _RimLightMul), _RimLightPow));

		o.Albedo = _Color.rgb;
		o.Alpha = saturate(_Alpha * RimLight);
	}

	fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
	{
		fixed4 c;
		c.rgb = s.Albedo + _Intensity;
		c.a = s.Alpha;
		return c;
	}

	ENDCG

	}
		FallBack ""
}

Shader "Custom/ReachableRegion"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Fade ("Fade", Range(0,1)) = 0
	}
	SubShader
	{
		Tags
		{
			"Queue"="Transparent" "RenderType"="Transparent"
		}
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input
		{
			float3 worldNormal;
			float3 viewDir;
		};

		half _Glossiness;
		fixed4 _Color;
		half _Fade;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			// Albedo comes from a texture tinted by color
			fixed4 c = _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = 0;
			o.Smoothness = _Glossiness * _Fade;

			const float fresnel = 1 - smoothstep(0, _Fade / .5 + .5, dot(IN.worldNormal, IN.viewDir));

			o.Alpha = c.a * fresnel;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
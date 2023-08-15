Shader "Custom/BoundaryClosenessShader"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_LHandPos ("Left Hand Pos", Vector) = (0,0,0)
		_RHandPos ("Right Hand Pos", Vector) = (0,0,0)
		_CircleThickness ("Circle Thickness", Float) = .02
		_CircleDiameter ("Circle Diameter", Float) = .1
		_CircleSmoothness ("Circle Smoothness", Float) = .01
		_Fade ("Fade", Float) = 2
	}
	SubShader
	{
		Tags
		{
			"RenderType"="Transparent"
		}
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha:blend

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
			float3 worldPos;
		};

		half _CircleThickness;
		half _CircleDiameter;
		half _CircleSmoothness;
		half _Fade;
		half3 _LHandPos;
		half3 _RHandPos;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
		// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			const float l_dist = distance(IN.worldPos, _LHandPos);
			const float r_dist = distance(IN.worldPos, _RHandPos);

			const float l_hand = (smoothstep(
				1 - _CircleThickness - _CircleSmoothness,
				1 - _CircleThickness,
				frac(l_dist / _CircleDiameter)
			) + smoothstep(
				_CircleSmoothness,
				0,
				frac(l_dist / _CircleDiameter)
			)) * clamp(1 - l_dist * _Fade, 0, 1);

			const float r_hand = (smoothstep(
				1 - _CircleThickness - _CircleSmoothness,
				1 - _CircleThickness,
				frac(r_dist / _CircleDiameter)
			) + smoothstep(
				_CircleSmoothness,
				0,
				frac(r_dist / _CircleDiameter)
			)) * clamp(1 - r_dist * _Fade, 0, 1);

			o.Albedo = _Color;
			o.Alpha = max(l_hand, r_hand) * _Color.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
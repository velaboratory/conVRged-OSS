// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/DistortedGrid" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_GridStrength ("Grid Strength", Range(0,1)) = 1.0
		_GridSize ("Grid Size", float) = 1
		_GridThickness("Grid Thickness", Range(0,1)) = .05
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;
			float3 wordNormal;
			float3 viewDir;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float _GridStrength;
		float _GridSize;
		float _GridThickness;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {		

			// create contour lines
			float d = 1;
			d = step(frac((IN.worldPos.x) * 1/_GridSize), 1-_GridThickness);
			d *= step(frac((IN.worldPos.z) * 1/ _GridSize), 1-_GridThickness);
			// reduce the effect
			d = lerp(1,d,_GridStrength);

			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color * d;

			o.Albedo = c.rgb;

			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}

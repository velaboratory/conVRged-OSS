Shader "ENGREDUVR/TriMixTerrain"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Main (RGB)", 2D) = "white" {}
		_MainTex_N("Main (Normal)", 2D) = "white" {}
		_SecondaryTex("Secondary (RGB)", 2D) = "white" {}
		_SecondaryTex_N("Secondary (Normal)", 2D) = "white" {}
		[MaterialToggle] _ErodedBool("Eroded", Float) = 0
		_ErodedTex("Eroded (RGB)", 2D) = "white" {}
		_ErodedTex_N("Eroded (Normal)", 2D) = "white" {}
		_MixMap("Mix Map (R and G)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		[MaterialToggle] _ShowGridBool("Show Grid", Float) = 0
		_GridBounds("Grid Bounds (x,y)", Vector) = (0,0,0)
		_GridBoundsCenter("Center of Grid Bounds (x,y)", Vector) = (0,0,0)
		_GridStrength("Grid Strength", Range(0,1)) = .2
		_GridSize("Grid Size", float) = 1
		_GridThickness("Grid Thickness", Range(0,1)) = .05
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;
			sampler2D _MainTex_N;
			sampler2D _SecondaryTex;
			sampler2D _SecondaryTex_N;
			sampler2D _ErodedTex;
			sampler2D _ErodedTex_N;
			sampler2D _MixMap;
			float _ErodedBool;
			float _ShowGridBool;
			float _GridStrength;
			float _GridSize;
			float _GridThickness;
			float2 _GridBounds;
			float2 _GridBoundsCenter;


			struct Input
			{
				float2 uv_MainTex;
				float2 uv_MixMap;
				float3 worldPos;
			};

			half _Glossiness;
			fixed4 _Color;

			// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
			// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
			// #pragma instancing_options assumeuniformscaling
			UNITY_INSTANCING_BUFFER_START(Props)
				// put more per-instance properties here
			UNITY_INSTANCING_BUFFER_END(Props)

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				// get the textures
				fixed4 c1 = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				fixed4 c2 = tex2D(_SecondaryTex, IN.uv_MainTex) * _Color;
				fixed4 c3 = tex2D(_ErodedTex, IN.uv_MainTex) * _Color;

				fixed4 n1 = tex2D(_MainTex_N, IN.uv_MainTex);
				fixed4 n2 = tex2D(_SecondaryTex_N, IN.uv_MainTex);
				fixed4 n3 = tex2D(_ErodedTex_N, IN.uv_MainTex);

				fixed4 mix = tex2D(_MixMap, IN.uv_MixMap);

				// apply the general mix
				fixed4 final_c = lerp(c1, c2, mix.x);
				fixed4 final_n = lerp(n1, n2, mix.x);

				// only use eroded map when bool is checked
				mix.y = mix.y * _ErodedBool;

				// apply the eroded map
				final_c = lerp(final_c, c3, mix.y);
				final_n = lerp(final_n, n3, mix.y);



				// create grid lines
				float d = 1;
				d = step(frac((IN.worldPos.x - _GridThickness * _GridSize / 2) / _GridSize), 1 - _GridThickness);
				d *= step(frac((IN.worldPos.z - _GridThickness * _GridSize / 2) / _GridSize), 1 - _GridThickness);
				// reduce the effect
				d = lerp(1, d, _GridStrength);

				// add the mask defined by the bounds
				fixed2 distance = (IN.worldPos.x - _GridBoundsCenter.x, IN.worldPos.z - _GridBoundsCenter.y);
				distance.x /= _GridBounds.x;
				distance.y /= _GridBounds.y;
				distance = saturate(saturate(distance.x) + saturate(distance.y));
				//d = d * distance;

				final_c = final_c * lerp(1, d, _ShowGridBool);



				// apply a global height-based fade
				//return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
				final_c = final_c * saturate((IN.worldPos.y - 5) / (15 - 5) * (1.5 - .5) + .5);
				


				
				o.Albedo = final_c.rgb;
				//o.Normal = UnpackNormal(final_n);
				o.Smoothness = _Glossiness;
			}
			ENDCG
		}
			FallBack "Diffuse"
}

Shader "ENGREDUVR/TargetAreaLines"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Fade" "Queue"="Transparent"}
		LOD 100
		//ZWrite Off
		Cull Off
		//Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard alpha:fade

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input
		{
			float3 worldPos;
			float2 uv_MainTex;
		};

		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			float3 localPos = IN.worldPos -  mul(unity_ObjectToWorld, float4(0,0,0,1)).xyz;
			float xScale = length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)); // scale x axis
			float yScale = length(float3(unity_ObjectToWorld[0].y, unity_ObjectToWorld[1].y, unity_ObjectToWorld[2].y)); // scale y axis


			// Albedo comes from a texture tinted by color
			fixed4 c = _Color;


			// fade alpha to top
			float alphaFade = localPos.y;
			alphaFade /= yScale;
			alphaFade = -alphaFade;
			alphaFade += 1;
			alphaFade /= 2;
			alphaFade *= frac(sin(100*(localPos.y-_Time)));

			o.Albedo = c.rgb * 20;
			o.Alpha = alphaFade * c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}

Shader "ENGREDUVR/BubbleLevel"
{
	Properties
	{
		_Color("Bubble Color", Color) = (1,1,1,1)
		_RingScale("Scale", float) = 0.5
		_BubbleScale("Bubble Scale", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_BubbleMoveSpeed("Bubble Move Sensitivity", float) = 1
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

		struct Input
		{
			float3 localPos;
			float3 worldPos;
			float3 worldNormal;
		};

		half _RingScale;
		half _BubbleScale;
		half _Metallic;
		half _Glossiness;
		half _BubbleMoveSpeed;

		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

			/*void vert(inout appdata_full v, out Input o) {
				UNITY_INITIALIZE_OUTPUT(Input, o);
				o.localPos = v.vertex.xyz;
			}*/

			void surf(Input IN, inout SurfaceOutputStandard o)
			{

			// RING
			float3 localPos = IN.worldPos - mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
			half localDistance = distance(localPos, float3(0, 0, 0));// *(1 - _RingScale);

			half outerRing = step(localDistance, .5 * _RingScale);
			half innerRing = step(localDistance, .4 * _RingScale);

			half ring = outerRing - innerRing;

			// Smaller Ring
			float3 localPos2 = IN.worldPos - mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
			half localDistance2 = distance(localPos2, float3(0, 0, 0));// *(1 - _RingScale);

			half outerRing2 = step(localDistance2, .235 * _RingScale);
			half innerRing2 = step(localDistance2, .21 * _RingScale);

			half ring2 = outerRing2 - innerRing2;


			// BUBBLE
			float3 norm = IN.worldNormal * _BubbleMoveSpeed;
			norm.x = clamp(norm.x, -.4, .4);
			norm.z = clamp(norm.z, -.4, .4);
			norm = norm * _RingScale + IN.worldPos;
			norm.y = 0;

			float4 objectOrigin = mul(unity_ObjectToWorld, float4(0.0, 0.0, 0.0, 1.0));
			objectOrigin.y = 0;
			objectOrigin.w = 0;
			half bubble = distance(objectOrigin, norm);
			bubble = step(bubble, _BubbleScale * _RingScale);
			bubble *= .9;


			// add both together
			fixed4 c = (1 - clamp(ring + bubble + ring2, 0, 1)) * _Color;
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

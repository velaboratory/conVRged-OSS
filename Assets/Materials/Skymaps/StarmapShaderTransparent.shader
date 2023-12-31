Shader "Unlit/StarmapShaderTransparent"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Exposure ("Exposure", Range(0,5)) = 1
	}
	SubShader
	{
		Tags
		{
			"QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent"
		}
		LOD 100

		Pass
		{
			Tags
			{
				"QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent"
			}
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID //Insert
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO //Insert
			};

			sampler2D _MainTex;
			float _Exposure;
			float4 _MainTex_ST;

			v2f vert(appdata v)
			{
				v2f o;
                UNITY_SETUP_INSTANCE_ID(v); //Insert
                UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o, o.vertex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * _Exposure;
				col.a = max(max(col.r, col.g), col.b);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
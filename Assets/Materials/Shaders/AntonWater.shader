Shader "Custom/AntonWater" {
	Properties {
	    // alpha is used from main color
		_Color1 ("Main Color (Alpha)", Color) = (0.54,0.76,0.84,.7)
		_Color2 ("Secondary Color", Color) = (1,1,1,1)
		_FresnelPower ("FresnelPower", Float) = .9
		_NormalStrength("Normal Strength", Float) = 1
		_Tex1 ("Texture 1", 2D) = "white" {}
		_Tex2 ("Texture 2", 2D) = "white" {}
		_TexRatioMin ("Texture Ratio Min", Range(0,1)) = .2
		_TexRatioMax ("Texture Ratio Max", Range(0,1)) = .5
		_Tex1SpeedX ("Texture 1 Speed X", Float) = .05 
		_Tex1SpeedY ("Texture 1 Speed Y", Float) = .05 
		_Tex2SpeedX ("Texture 2 Speed X", Float) = .05 
		_Tex2SpeedY ("Texture 2 Speed Y", Float) = .05
		_HeightMult ("Height Multiplier", Float) = 1
		_Metallic ("Metallic", Range(0,1)) = .15
		_Smoothness ("Smoothness", Range(0,1)) = 1
		_Alpha ("Alpha", Range(0,1)) = .5
	}
	SubShader {
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 100
		
		
        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha
        #pragma vertex vert
        //#pragma fragment frag

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0
        #include "UnityCG.cginc"

        struct appdata {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
            
        };
        
        struct v2f {
            float2 uv : TEXCOORD0;
            float4 vertex : SV_POSITION;
        };
        
        struct Input {
            float2 uv_Tex1;
            float2 uv_Tex2;
            float3 viewDir;
        };
        
        fixed4 _Color1;
        fixed4 _Color2;
        float _FresnelPower;
        float _NormalStrength;
        sampler2D _Tex1;
        //float4 _Tex1_ST;
        sampler2D _Tex2;
        //float4 _Tex2_ST;
        float _TexRatioMin;
        float _TexRatioMax;
        float _Tex1SpeedX;
        float _Tex1SpeedY;
        float _Tex2SpeedX;
        float _Tex2SpeedY;
        float _Metallic;
        float _Smoothness;
        float _HeightMult;
        float _Alpha;
        
        void vert (inout appdata_full v) {
            v.vertex.z += sin(v.vertex.x/.00001 + _Time*50) * .00004 * _HeightMult;
        }

        void surf(Input IN, inout SurfaceOutputStandard o) {
            // generate offsets
            float offset1X = _Time * _Tex1SpeedX;
            float offset1Y = _Time * _Tex1SpeedY;
            float offset2X = _Time * _Tex2SpeedX;
            float offset2Y = _Time * _Tex2SpeedY;
            fixed2 offset1 = fixed2(offset1X, offset1Y);
            fixed2 offset2 = fixed2(offset2X, offset2Y);
            
            // apply offsets
            fixed2 tex1UV = IN.uv_Tex1 + offset1;
            fixed2 tex2UV = IN.uv_Tex2 + offset2;
            
            // generate lerp between textures
            float lerpVal = (sin(_Time*10) + 1)/2  * (_TexRatioMax - _TexRatioMin) + _TexRatioMin;
            
            // fresnel
            half factor = dot(normalize(IN.viewDir), o.Normal);
            fixed4 FinalColor = lerp(_Color1, _Color2, factor);
            
            
            float4 tex1Pixel = tex2D(_Tex1, tex1UV);
            float4 tex2Pixel = tex2D(_Tex2, tex2UV);
            float4 pixel = lerp(tex1Pixel, tex2Pixel, lerpVal);
            float3 n = UnpackScaleNormal(pixel, _NormalStrength);
            
            o.Normal = n.xyz;
            o.Albedo = FinalColor.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Smoothness;
            // alpha comes from color1
            o.Alpha = _Alpha;
        }
        
        ENDCG
	}
	FallBack "Diffuse"
}

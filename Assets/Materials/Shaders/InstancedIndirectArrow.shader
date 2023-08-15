Shader "Custom/InstancedIndirectArrow" {
    SubShader {
        Tags { "RenderType" = "Opaque" }

        Pass {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            struct appdata_t {
                float4 vertex   : POSITION;
            };

            struct v2f {
                float4 vertex   : SV_POSITION;
                float depth : BLENDWEIGHTS0;
            }; 

            struct MeshProperties {
                float4x4 mat;
            };

            StructuredBuffer<MeshProperties> _Properties;
            float4x4 _testest;
            float _waterTop;
            float _lengthScale;
            fixed4 _arrowColor;
            v2f vert(appdata_t i, uint instanceID: SV_InstanceID) {
                v2f o;
                float4 tipPos = mul(_Properties[instanceID].mat,float4(0,0,0,1));
                tipPos = mul(_testest,tipPos);
                float depth = max(0,_waterTop-tipPos.y); //now we know the depth, we can scale the vertex accordingly
                float4 v = i.vertex;
                v.z = v.z*depth*_lengthScale;
                float4 pos = mul(_Properties[instanceID].mat, v);
                pos = mul(_testest,pos); //takes you to world space
                o.vertex = UnityWorldToClipPos(pos);
                o.depth = depth;

                return o;
            }
            
            fixed4 frag(v2f i) : SV_Target {
                if(i.depth <= 0) discard;
                return _arrowColor;
            }
            
            ENDCG
        }
    }
}
// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/TestGeomShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WaterTop("WaterTop",Vector) = (0,0,0,1)
        _TipColor("Tip Color", Color) = (1,0,0,1)
        _ShaftColor("Shaft Color",Color) = (0,0,1,1)
        _Depth2Pressure("Depth2Pressure",float) = .1
        _HeadMin("HeadMinimu Dist", float) = .3
         
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID //Insert
            };

            struct v2g
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                UNITY_VERTEX_OUTPUT_STEREO //Insert
            };

            struct g2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                UNITY_VERTEX_OUTPUT_STEREO //Insert
            };


            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _WaterTop;
            float4 _TipColor;
            float4 _ShaftColor;
            float _Depth2Pressure;
            float _HeadMin;
            v2g vert (appdata v)
            {
                v2g o;
                
                UNITY_SETUP_INSTANCE_ID(v); //Insert
                UNITY_INITIALIZE_OUTPUT(v2g, o); //Insert
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert
                o.vertex = v.vertex;
                o.uv = v.uv;
                o.normal = v.normal;
                return o;
            }

            //helper func to draw a triangle in the geometer shader
            void drawTriangle(float4 v2, float4 v1, float4 v0, float4 color, inout TriangleStream<g2f> triStream){
                g2f o;
                UNITY_INITIALIZE_OUTPUT(g2f, o); //Insert
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert
                o.uv = float2(0,0);
                
                o.color = color;
                o.vertex = v2;
                triStream.Append(o);

                o.vertex = v1;
                triStream.Append(o);
                
                o.vertex = v0;
                triStream.Append(o);
                triStream.RestartStrip();

            }

            [maxvertexcount(36)]
            void geom(triangle v2g IN[3], inout TriangleStream<g2f> triStream)
            {
                
                float3 averageNormal = float3(0,0,0);
                float4 averagePos = float4(0,0,0,1);

                //start by calculating the face normal and center position from the inputs
                for(int i = 0; i < 3; i++)
                {
                    averageNormal = averageNormal + IN[i].normal;
                    averagePos.xyz = averagePos.xyz + IN[i].vertex.xyz;
                    
                }
                averageNormal = averageNormal/3;
                averagePos.xyz = averagePos.xyz/3;
                //averagePos.w = 1; //need to do this, otherwise it's all broken because W is scaled (probably smarter way)
                
                averageNormal = UnityObjectToWorldNormal(averageNormal);

                //you need to convert to world position to measure against the water (or convert water level to object)
                float4 averagePosWorld = mul(unity_ObjectToWorld,averagePos);
                float pressure = _Depth2Pressure*max(_WaterTop.y - averagePosWorld.y, 0); //now you can find the pressure easily 
                float3 tailStart = averageNormal*min(pressure,_HeadMin);
                float3 tailEnd = averageNormal*pressure;
        
                if(pressure == 0){ //if no pressure, just don't draw anything
                    return;
                }
                
                float4 in0World = mul(unity_ObjectToWorld,IN[0].vertex);
                float4 in1World = mul(unity_ObjectToWorld,IN[1].vertex);
                float4 in2World = mul(unity_ObjectToWorld,IN[2].vertex);
            
                
                float4 v0 = UnityWorldToClipPos(averagePosWorld + tailStart + (in0World-averagePosWorld) *.1);
                float4 v1 = UnityWorldToClipPos(averagePosWorld + tailStart + (in1World-averagePosWorld) *.1);
                float4 v2 = UnityWorldToClipPos(averagePosWorld + tailStart + (in2World-averagePosWorld) *.1);

                float4 v3 = UnityWorldToClipPos(averagePosWorld + tailStart + (in0World-averagePosWorld) *.05);
                float4 v4 = UnityWorldToClipPos(averagePosWorld + tailStart + (in1World-averagePosWorld) *.05);
                float4 v5 = UnityWorldToClipPos(averagePosWorld + tailStart + (in2World-averagePosWorld) *.05);

                float4 v6 = UnityWorldToClipPos(averagePosWorld + tailEnd + (in0World-averagePosWorld) *.05);
                float4 v7 = UnityWorldToClipPos(averagePosWorld + tailEnd + (in1World-averagePosWorld) *.05);
                float4 v8 = UnityWorldToClipPos(averagePosWorld + tailEnd + (in2World-averagePosWorld) *.05);

                float4 vTip = UnityWorldToClipPos(averagePosWorld);
                
                //draw tip (4 triangles)
                
                drawTriangle(vTip,v1,v0, _TipColor,triStream);
                drawTriangle(vTip,v2,v1, _TipColor,triStream);
                drawTriangle(vTip,v0,v2, _TipColor,triStream);
                drawTriangle(v0,v1,v2, _TipColor,triStream);

                //draw shaft (8 triangles)
                drawTriangle(v3,v4,v5, _ShaftColor,triStream); //start
                drawTriangle(v6,v7,v8, _ShaftColor,triStream); //end
                
                drawTriangle(v3,v4,v6, _ShaftColor,triStream);
                drawTriangle(v4,v7,v6, _ShaftColor,triStream);

                drawTriangle(v4,v5,v7, _ShaftColor,triStream);
                drawTriangle(v5,v8,v7, _ShaftColor,triStream);

                drawTriangle(v5,v3,v8, _ShaftColor,triStream);
                drawTriangle(v3,v6,v8, _ShaftColor,triStream);

            }

            fixed4 frag (g2f i) : SV_Target
            {
                fixed4 col = i.color;
                return col;
            }
            ENDCG
        }
    }
}

Shader "Game/Units/UnitHealthShader"
{
 Properties
    {
        _FillColor ("Fill Color", Color) = (1,1,1,0.1)
        _BorderWidth ("Border Widt", float) = 1.0
        _BorderColor ("Border Color", Color) = (1,1,1,0.1)
        _Smoothness ("Smoothness", float) = 0.05
        _BorderMax ("Max Border", float) = 0.1
        _QuantityForOneCell ("Quantity for one cell", float) = 5.0
        _CellWidth ("Cell Width", float) = 0.1
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
        }
        ZWrite Off
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            
            #include "UnityCG.cginc"
            #define UNITY_INDIRECT_DRAW_ARGS IndirectDrawIndexedArgs
            #include "UnityIndirect.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float fill : fixed4;
            };

            UNITY_INSTANCING_BUFFER_START(Properties)
            UNITY_DEFINE_INSTANCED_PROP(float, _Fill)
            UNITY_INSTANCING_BUFFER_END(Properties)

            sampler2D _MainTex;
            float2 _MeshSize;
            
            CBUFFER_START(UnityPerMaterial)
            float _BorderMax;
            float _CellWidth;
            float _Smoothness;
            float4 _FillColor;
            float _BorderWidth;
            float4 _BorderColor;
            float _QuantityForOneCell;
            CBUFFER_END

            v2f vert(appdata v)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                float fill = UNITY_ACCESS_INSTANCED_PROP(Properties, _Fill);
                
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.fill = fill;
                
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float2 size = _MeshSize;
                float borderWidth = _BorderWidth;
                
                float borderX = borderWidth / size.x;
                float borderY = borderWidth / size.y;

                float distX = min(i.uv.x, 1.0 - i.uv.x);
                float distY = min(i.uv.y, 1.0 - i.uv.y);
                
                float distXNorm = distX / borderX;
                float distYNorm = distY / borderY;
                float dist = min(distXNorm, distYNorm);

                float ddg = max(ddx(i.uv.x), ddx(i.uv.y)) * -_Smoothness;
                
                float borderBlend = smoothstep(_BorderMax - ddg, _BorderMax, dist);
                
                float cellInterval = 1.0 / _QuantityForOneCell;
                float cellWidthUV = _CellWidth / size.x;
                
                float cellPos = i.uv.x / cellInterval;
                float cellFraction = frac(cellPos);
                
                float cellBorder = smoothstep(cellWidthUV, 0.0, cellFraction) + 
                                  smoothstep(1.0 - cellWidthUV, 1.0, cellFraction);
                cellBorder = saturate(cellBorder);
                
                float healthThreshold = i.uv.x;
                float healthMask = step(healthThreshold, i.fill);
                
                half4 fillArea = _FillColor * healthMask; 
                half4 borderArea = _BorderColor * saturate(borderBlend + cellBorder); 
                
                half4 result = lerp(fillArea, borderArea, borderArea.a);
                
                result.a = max(fillArea.a * healthMask, borderArea.a);
                
                return result;
            }
            ENDCG
        }
    }
}
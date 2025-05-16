Shader "Game/Units/MouseSelectionShader"
{
   Properties
    {
        _FillColor ("Fill Color", Color) = (1,1,1,0.1)
        _BorderColor ("Border Color", Color) = (1,0,0,1)
        _BorderThicknessPx ("Border Thickness (px)", Range(1,10)) = 3
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            fixed4 _FillColor;
            fixed4 _BorderColor;
            float _BorderThicknessPx;
            float2 _QuadSize;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float borderU = _BorderThicknessPx / _QuadSize.x;
                float borderV = _BorderThicknessPx / _QuadSize.y;
                
                if (i.uv.x < borderU || i.uv.x > 1.0 - borderU || i.uv.y < borderV || i.uv.y > 1.0 - borderV)
                    return _BorderColor;
                
                return _FillColor;
            }
            ENDCG
        }
    }
    FallBack "Unlit/Transparent"
}
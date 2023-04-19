Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex ("Cloud Colour", 2D) = "white" {}
        _MaskTex ("Cloud Mask", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

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
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            sampler2D _MaskTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv.xyyy;
                o.uv.z -= _Time.x*3;
                o.uv.w -= _Time.x*2;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv.xy);
                fixed maskR = tex2D(_MaskTex, i.uv.xy).r;
                fixed maskG = tex2D(_MaskTex, i.uv.xz).g;
                fixed maskB = tex2D(_MaskTex, i.uv.xw).b;

                fixed A = maskR*maskG*maskB*3;
                col.a = A*i.color.a;
                return col;
            }
            ENDCG
        }
    }
}

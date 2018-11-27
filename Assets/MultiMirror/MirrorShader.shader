Shader "Unlit/Mirror"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", COLOR) = (1,1,1,1)
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            struct v2f {};

            v2f vert (float4 vertex : POSITION, out float4 outpos : SV_POSITION ){
                v2f o;
                outpos = UnityObjectToClipPos(vertex);
                return o;
            }

            sampler2D _MainTex;
            float4 _Color;

            fixed4 frag (v2f i, UNITY_VPOS_TYPE screenPos : VPOS) : SV_Target
            {
            	screenPos.x = 1 - screenPos.x/_ScreenParams.x;
            	screenPos.y = screenPos.y/_ScreenParams.y;
                return tex2D(_MainTex, screenPos.xy)* _Color;
            }
            ENDCG
        }
    }
}
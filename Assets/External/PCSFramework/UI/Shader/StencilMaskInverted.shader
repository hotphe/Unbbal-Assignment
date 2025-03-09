Shader "UI/StencilMaskInverted"
{
    Properties
    {
        _MainTex("Base (RGB)", 2D) = "white" {} // ����ũ�� ������ �̹���
        _Color("Color", Color) = (1,1,1,1)
    }
        SubShader
        {
            Tags { "Queue" = "Overlay" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
            Stencil
            {
                Ref 9987 // ���ٽ� ���� ��
                Comp always // �׻� ���� ���� ���ٽ� ���ۿ� ���
                Pass replace // �н� �� ���ٽ� ���� ���� ���� ������ ��ü
            }
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask 0 // ������ ������� ����

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct appdata_t
                {
                    float4 vertex : POSITION;
                    float2 texcoord : TEXCOORD0;
                };

                struct v2f
                {
                    float4 vertex : SV_POSITION;
                    float2 texcoord : TEXCOORD0;
                };

                v2f vert(appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.texcoord = v.texcoord;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    return fixed4(0, 0, 0, 0); // ���� ���� (����ũ�� �����ϹǷ� ��� ����)
                }
                ENDCG
            }
        }
            Fallback "Transparent/VertexLit"
}

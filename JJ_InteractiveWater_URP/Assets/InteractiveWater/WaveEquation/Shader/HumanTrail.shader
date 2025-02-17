Shader "Custom/HumanTrail"
{
    Properties
    {
        _MainTex ("_MainTex", 2D) = "black" {}
        _HumanPosTex ("_HumanPosTex", 2D) = "black" {}
        _HumanSpeed ("_HumanSpeed", Float) = 0
        _uvOffset("_uvOffset",Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalRenderPipeline" }
        
        ZWrite Off
        ZTest Always
        Cull Off
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            struct my_PlayerStruct
            {
                float speed;
                float3 PositionWS;
                float2 uvOffset;
            };

            sampler2D _MainTex;
            sampler2D _HumanPosTex;
            float _HumanSpeed;
            half2 _uvOffset;
            StructuredBuffer<my_PlayerStruct> _my_PlayerBuffer;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
               
                // 读取当前轨迹和新的位置信息
                half oldTrail = tex2D(_MainTex, i.uv - _uvOffset).r;
                half newTrail = tex2D(_HumanPosTex, i.uv).r;

                // 衰减旧的轨迹
                oldTrail *= 0.95;
                newTrail *= _HumanSpeed ;
                newTrail = saturate(newTrail);

                // 将新的位置数据添加到轨迹中
                half finalTrail = lerp(oldTrail , 1 , newTrail);

                return half4(finalTrail, 0, 0, 1);
            }
            ENDHLSL
        }
    }
}

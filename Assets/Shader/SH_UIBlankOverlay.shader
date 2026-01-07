Shader "Custom/SH_UI_PurpleMask_SinglePass"
{
    Properties
    {
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {}
        _Sensitivity("Purple Sensitivity", Range(0.1, 1)) = 0.5
    }

    SubShader
    {
        Tags 
        { 
            "RenderPipeline"="UniversalPipeline" 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
        }

        // -----------------------------------------------------------
        // 핵심: 하나의 Pass에서 "그리기"와 "스텐실 작성"을 동시에 수행
        // -----------------------------------------------------------
        Pass
        {
            Name "MainRender"
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            
            // ★ 스텐실 설정: 무조건 통과시키고, 그 자리에 1을 쓴다.
            // 픽셀 셰이더에서 discard(버림) 되는 픽셀은 스텐실도 안 쓰입니다.
            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings { float4 positionHCS : SV_POSITION; float2 uv : TEXCOORD0; };

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            float4 _BaseMap_ST; 
            half _Sensitivity;

            Varyings vert(Attributes IN) {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target {
                half4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                
                // 1. 투명한 배경(알파값 0)은 아예 그리지 않음 -> 스텐실도 안 찍힘 (중요)
                if (col.a < 0.1) discard;

                // 색상 판별 로직
                bool isPurple = (col.r > _Sensitivity && col.b > _Sensitivity && col.g < 0.4);
                
                // 2. 보라색인 경우:
                // 화면에는 "투명"하게 그려야 함 (구멍). 
                // 하지만 discard를 쓰면 안 됨! discard하면 스텐실(1)이 안 찍힘.
                // 대신 alpha를 0으로 만들어 반환함.
                if (isPurple) 
                {
                    return half4(0, 0, 0, 0); 
                }

                // 3. 흰색 선인 경우:
                // 원래 색상을 그대로 반환. (스텐실 1도 같이 찍힘)
                return col;
            }
            ENDHLSL
        }
    }
}
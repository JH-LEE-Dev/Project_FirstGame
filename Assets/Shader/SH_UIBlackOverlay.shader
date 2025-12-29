Shader "Custom/SH_UIBlackOverlay"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white" {} // {} 추가 (에러 방지)
    }

    SubShader
    {
        // 투명 처리를 위해 Queue와 RenderType 수정
        Tags 
        { 
            "RenderPipeline"="UniversalPipeline" 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
        }

        // --- 스텐실 설정 추가 ---
        Stencil
        {
            Ref 1           // 비교할 기준 값 (1)
            Comp NotEqual   // 스텐실 값이 1이 "아닌" 픽셀만 그린다 (1이면 투명/통과)
            Pass Keep       // 스텐실 값은 변경하지 않고 유지
        }
        // -----------------------

        Pass
        {
            // 투명 셰이더 기본 설정
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // 텍스처 컬러 샘플링
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;
                return color;
            }
            ENDHLSL
        }
    }
}
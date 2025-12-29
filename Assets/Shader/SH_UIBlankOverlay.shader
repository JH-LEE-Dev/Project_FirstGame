Shader "Custom/SH_UI_PurpleMask_Fixed"
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
        // PASS 1: 흰색 선만 화면에 그리기 (보라색은 제외)
        // -----------------------------------------------------------
        Pass
        {
            Name "MainRender"
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ColorMask RGB      // 여기서는 색상을 그림

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings { float4 positionHCS : SV_POSITION; float2 uv : TEXCOORD0; };

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            float4 _BaseMap_ST; half _Sensitivity;

            Varyings vert(Attributes IN) {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target {
                half4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                bool isPurple = (col.r > _Sensitivity && col.b > _Sensitivity && col.g < 0.4);

                // 보라색인 부분은 화면에서 그리지 않음 (흰색 선만 남음)
                if (isPurple) discard;

                return col;
            }
            ENDHLSL
        }

        // -----------------------------------------------------------
        // PASS 2: 보라색 영역을 찾아서 스텐실만 '먼저' 기록 (화면엔 안 그림)
        // -----------------------------------------------------------
        Pass
        {
            Name "StencilWrite"
            ColorMask 0
            ZWrite Off
            
            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace   // 보라색 위치에 1 기록
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings { float4 positionHCS : SV_POSITION; float2 uv : TEXCOORD0; };

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            float4 _BaseMap_ST; half _Sensitivity;

            Varyings vert(Attributes IN) {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target {
                half4 col = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                bool isWhiteLine = (col.r > 0.9 && col.g > 0.9 && col.b > 0.9);

                if (isWhiteLine) 
                  discard;
                else
                {
                    if(col.a == 0)
                        discard;
                }

                return 0;
            } 

            ENDHLSL
        }
    }
}
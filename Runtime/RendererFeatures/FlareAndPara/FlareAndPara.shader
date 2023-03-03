Shader "Hidden/AnimeStylePostProcess/FlareAndPara"
{
    HLSLINCLUDE
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "../../ShaderLibrary/YmUtility.hlsl"

    half _FlareIntensity;
    half _FlareInteration;
    half3 _FlareInnerColor;
    half3 _FlareOuterColor;
    half _ColorMixedMidPoint;
    half _ColorMixedSoftness;

    float _FlareRange;
    half _RotateWithMainLightAngle;

    half2 _ControlPoint;
    half2 _StartPosition;
    half2 _EndPosition;

    
    half _ParaIntensity;
    half _ParaRange;
    half _ParaRotation;

    
    struct Attributes
    {
        float4 positionOS : POSITION;
        float2 uv : TEXCOORD0;
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        half3 color : TEXCOORD0;
        half4 vary : TEXCOORD1;
    };

    
    Varyings FlareVert(Attributes input)
    {
        input.positionOS.y += 1.5;
        input.positionOS.y -= _FlareRange;
        input.positionOS.x *= 2;
        
        if (input.uv.y > 0)
        {
            input.positionOS.y += 1;
            input.uv.y += 1;
        }

        Varyings output = (Varyings)0;
        
        Unity_Rotate_Degrees_float(input.positionOS.xy, float2(0, 0), _RotateWithMainLightAngle, input.positionOS.xy);
        output.positionCS = TransformObjectToHClip(input.positionOS);

        output.vary.x = max(0, input.uv.y * 2 - 1);

        return output;
    }

    half4 FlareFrag(Varyings input) : SV_Target
    {
        Light mainLight = GetMainLight();
        float3 lightColor = mainLight.color * 0.8;
        half smoothColorSplit = smoothstep(1 - _ColorMixedMidPoint - _ColorMixedSoftness,
                                           1 - _ColorMixedMidPoint + _ColorMixedSoftness, input.vary.x);
        half3 flare = lerp(_FlareOuterColor, _FlareInnerColor, smoothColorSplit) * lightColor * _FlareIntensity * input.vary.x;

        return half4(pow(saturate(flare),_FlareInteration), 1);
    }

    Varyings ParaVert(Attributes input)
    {
        input.positionOS.y -= 5;
        input.positionOS.y += _ParaRange;
        input.positionOS.x *= 2;

        Varyings output = (Varyings)0;

        Unity_Rotate_Degrees_float(input.positionOS.xy, float2(0, 0), _ParaRotation, input.positionOS.xy);
        output.positionCS = TransformObjectToHClip(input.positionOS);

        output.color = lerp(1,input.uv.y,(0.5-input.uv.y)*_ParaIntensity);
        // output.vary.x = input.uv.y;

        return output;
    }

    half4 ParaFrag(Varyings input) : SV_Target
    {
        // half3 para = lerp(1, input.vary.x, _ParaIntensity);
        // return half4(para, 1);

        return half4(input.color*input.color,1);
    }
    ENDHLSL

    SubShader
    {
        ZTest Off
        ZWrite Off
        Cull Off

        Pass
        {
            Name "AnimeStylePostProcess Flare"

            Blend OneMinusDstColor One

            HLSLPROGRAM
            #pragma enable_d3d11_debug_symbols
            #pragma vertex FlareVert
            #pragma fragment FlareFrag
            
            ENDHLSL
        }
        
        Pass{
            Name "AnimeStylePostProcess Para"

            Blend DstColor Zero

            HLSLPROGRAM
            #pragma enable_d3d11_debug_symbols
            #pragma vertex ParaVert
            #pragma fragment ParaFrag
            
            ENDHLSL
            
            }
    }
}
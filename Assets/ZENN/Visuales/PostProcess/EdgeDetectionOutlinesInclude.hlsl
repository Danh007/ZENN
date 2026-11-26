#ifndef SOBELOUTLINES_INCLUDE
#define SOBELOUTLINES_INCLUDE

#ifndef SHADERGRAPH_PREVIEW
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"
#endif

static float2 sobelSamplePoint[9] = 
{
    float2(-1.0,  1.0), float2(0.0,  1.0), float2(1.0,  1.0),
    float2(-1.0,  0.0), float2(0.0,  0.0), float2(1.0,  0.0),
    float2(-1.0, -1.0), float2(0.0, -1.0), float2(1.0, -1.0)
};

static float sobelXMatrix[9] = 
{
     1.0, 0.0, -1.0,
     2.0, 0.0, -2.0,
     1.0, 0.0, -1.0
};

static float sobelYMatrix[9] = 
{
     1.0,  2.0,  1.0,
     0.0,  0.0,  0.0,
    -1.0, -2.0, -1.0
};

void DepthSobel_float(float2 UV, float Thickness, out float Out)
{
    float2 sobel = 0;

    #ifndef SHADERGRAPH_PREVIEW
        float2 texelSize = _CameraDepthTexture_TexelSize.xy;

        [unroll]
        for (int i = 0; i < 9; i++)
        {
            float2 sampleUV = UV + sobelSamplePoint[i] * Thickness * texelSize;
            float depth = SampleSceneDepth(sampleUV);
            depth = Linear01Depth(depth, _ZBufferParams);
            sobel += depth * float2(sobelXMatrix[i], sobelYMatrix[i]);
        }
    #endif

    Out = length(sobel);
}

void ColorSobel_float(float2 UV, float Thickness, out float Out)
{
    float2 sobelR = 0;
    float2 sobelG = 0;
    float2 sobelB = 0;

    #ifndef SHADERGRAPH_PREVIEW
        // Use _ScreenParams.xy instead of _CameraOpaqueTexture_TexelSize
        float2 texelSize = 1.0 / _ScreenParams.xy;

        [unroll]
        for (int i = 0; i < 9; i++)
        {
            float2 sampleUV = UV + sobelSamplePoint[i] * Thickness * texelSize;
            
            // Sample _BlitTexture directly (automatically provided by Full Screen Pass)
            float3 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, sampleUV).rgb;
            
            float2 kernel = float2(sobelXMatrix[i], sobelYMatrix[i]);
            sobelR += color.r * kernel;
            sobelG += color.g * kernel;
            sobelB += color.b * kernel;
        }
    #endif

    Out = max(length(sobelR), max(length(sobelG), length(sobelB)));
}


#endif

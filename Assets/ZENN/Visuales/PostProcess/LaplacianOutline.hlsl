#ifndef LAPLACIAN_OUTLINE_INCLUDED
#define LAPLACIAN_OUTLINE_INCLUDED

void ComputeLaplacianOutline_float(
    float2 UV,
    float Thickness,
    float DepthSensitivity,
    float NormalSensitivity,
    float EdgeThreshold,
    out float EdgeMask)
{
    float2 texelSize = _ScreenParams.zw - 1.0;
    float2 offset = texelSize * Thickness;

    float2 offsets[9] = {
        float2(-offset.x,  offset.y), float2(0.0,  offset.y), float2(offset.x,  offset.y),
        float2(-offset.x,  0.0),      float2(0.0,  0.0),      float2(offset.x,  0.0),
        float2(-offset.x, -offset.y), float2(0.0, -offset.y), float2(offset.x, -offset.y)
    };

    float weights[9] = {
        -1.0, -1.0, -1.0,
        -1.0,  8.0, -1.0,
        -1.0, -1.0, -1.0
    };

    float depthLaplacian = 0.0;
    float3 normalLaplacian = float3(0.0, 0.0, 0.0);

    // Sample center linear eye depth using Shader Graph's built-in helper
    float centerRawDepth = SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV);
    float centerEyeDepth = LinearEyeDepth(centerRawDepth, _ZBufferParams);

    [unroll]
    for (int i = 0; i < 9; i++)
    {
        float2 sampleUV = UV + offsets[i];

        // 1. Depth Laplacian
        float rawDepth = SHADERGRAPH_SAMPLE_SCENE_DEPTH(sampleUV);
        float eyeDepth = LinearEyeDepth(rawDepth, _ZBufferParams);
        depthLaplacian += eyeDepth * weights[i];

        // 2. Normal Laplacian
        float3 worldNormal = SHADERGRAPH_SAMPLE_SCENE_NORMAL(sampleUV);
        normalLaplacian += worldNormal * weights[i];
    }

    float depthEdge = abs(depthLaplacian) / max(centerEyeDepth, 0.0001);
    depthEdge *= DepthSensitivity;

    float normalEdge = length(normalLaplacian) * NormalSensitivity;

    float combinedEdge = max(depthEdge, normalEdge);
    EdgeMask = step(EdgeThreshold, combinedEdge);
}

#endif
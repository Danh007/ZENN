#ifndef CEL_LIGHTNESS_OUTLINE_INCLUDED
#define CEL_LIGHTNESS_OUTLINE_INCLUDED

void ComputeCelLightnessOutline_float(
    float2 UV,
    float CelLightness,
    float Thickness,
    float LightnessSensitivity,
    float EdgeThreshold,
    out float LightnessEdgeMask)
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

    // Soft-compress center Lightness in case values exceed 1.0 (HDR)
    float centerLightness = max(CelLightness, 0.0);
    float centerCompressed = centerLightness / (1.0 + centerLightness);
    float lightnessLaplacian = centerCompressed * weights[4];

    [unroll]
    for (int i = 0; i < 9; i++)
    {
        if (i == 4) continue;

        float2 neighborUV = UV + offsets[i];
        float neighborRaw = SHADERGRAPH_SAMPLE_SCENE_DEPTH(neighborUV);
        // If you sample scene color for neighbor lightness, compress it the same way:
        float3 neighborRGB = SHADERGRAPH_SAMPLE_SCENE_COLOR(neighborUV).rgb;
        float neighborLightness = max(max(neighborRGB.r, neighborRGB.g), neighborRGB.b);
        float neighborCompressed = neighborLightness / (1.0 + neighborLightness);

        lightnessLaplacian += neighborCompressed * weights[i];
    }

    float lightnessEdge = abs(lightnessLaplacian) * LightnessSensitivity;
    LightnessEdgeMask = step(EdgeThreshold, lightnessEdge);
}

#endif
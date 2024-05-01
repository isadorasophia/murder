// Heavily inspired by https://colececil.io/blog/2017/scaling-pixel-art-without-destroying-it/
#include "murder.fxh"

float2 viewportSize;
float2 textureSize;
float texelsScale;

// Define the two colors for the checkerboard
float4 colorA = float4(1.0, 0.0, 0.0, 1.0);  // Red
float4 colorB = float4(0.0, 0.0, 0.3, 1.0);  // Blue

float4 MainPixelShader(VSOutput input) : SV_Target0
{
    float2 texelSize = float2(texelsScale, texelsScale);

    // Position within the input texture
   	float2 texPos = float2(input.uv * textureSize);
    
    // Position inside the input texture pixel
    float2 locationWithinTexel = frac(texPos);
    float2 interpolationAmount = clamp(locationWithinTexel / texelsScale, 0,
        .5) + clamp((locationWithinTexel - 1) / texelsScale + .5, 0, .5);
    
    float2 finalTextureCoords = (floor(input.uv * textureSize) +
        interpolationAmount) / texelSize.xy;

    float4 color = input.color;
    color.rg = locationWithinTexel;
    color.b = 0;
    return SAMPLE_TEXTURE(Texture, finalTextureCoords / viewportSize) * input.color;
}

float4 PixelMainPixelShader(VSOutput input) : SV_Target0
{
    float2 texelSize = float2(1 / textureSize.x, 1 / textureSize.y);

   	float2 locationWithinTexel = frac(input.uv * textureSize);
    float2 interpolationAmount = clamp(locationWithinTexel / texelsScale, 0,
        .5) + clamp((locationWithinTexel - 1) / texelsScale + .5, 0, .5);
    float2 finalTextureCoords = (floor(input.uv * textureSize) +
        interpolationAmount) / texelSize.xy;

    return SAMPLE_TEXTURE(Texture, finalTextureCoords / textureSize) * input.color;
}

SINGLE_PASS_TECHNIQUE
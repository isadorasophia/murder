// Heavily inspired by https://colececil.io/blog/2017/scaling-pixel-art-without-destroying-it/
#include "murder.fxh"

float2 textureSize;
float texelsScale;

float4 MainPixelShader(VSOutput input) : SV_Target0
{
    // Position within the input texture
    float2 texPos = input.uv * textureSize;
    
    // Position inside the input texture pixel
    float2 locationWithinTexel = frac(input.uv * textureSize);
    
    float2 interpolationAmount = clamp(locationWithinTexel / texelsScale, 0.0,
        0.5) + clamp((locationWithinTexel - 1) / texelsScale + 0.5, 0.0, 0.5);
    
    float2 finalTextureCoords = (floor(input.uv * textureSize) +
        interpolationAmount) / textureSize;
    
    return SAMPLE_TEXTURE(Texture, finalTextureCoords) * input.color;
}


SINGLE_PASS_TECHNIQUE
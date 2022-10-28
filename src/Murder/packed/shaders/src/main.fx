#include "Common.fxh"

#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif

//-----------------------------------------------------------------------------
// Globals.
//-----------------------------------------------------------------------------

float4x4 MatrixTransform;
sampler inputTexture;
int colorGradeSize;
float2 screenSize;
float2 cameraPosition;
float ditherAmount;
float2 pixelScale;
int ditherImageSize;
float shadeAmount;

float time;

DECLARE_TEXTURE(ditherTexture, 1)
{
    AddressU = Wrap;
    AddressV = Wrap;
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
};

DECLARE_TEXTURE(lightMap, 2)
{
    AddressU = Clamp;
    AddressV = Clamp;
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
};

// THis goes out of your Vertex Shader into your Pixel Shader
struct VSOutput
{
    float4 position     : SV_Position;
    float2 texCoord0    : TEXCOORD0;
};


//-----------------------------------------------------------------------------
// Helpers.
//-----------------------------------------------------------------------------

// based on https://godotshaders.com/shader/ps1-post-processing/
int DitheringPattern(float2 fragcoord)
{
    int x = (fragcoord.x) % ditherImageSize;
    int y = (fragcoord.y) % ditherImageSize;
    
    return (SAMPLE_TEXTURE(ditherTexture, float2(x,y)/ditherImageSize).r - 0.5) * 4;
}

float Multiply(float target, float blend)
{
    return target * blend;
}

float Overlay(float target, float blend)
{
    return
        step(target, 0.5) * (1 - (1 - 2 * (target - 0.5)) * (1 - blend)) +
        (1 - step(target, 0.5)) * ((2 * target) * blend);
}

float HardLight(float target, float blend)
{
    return
        step(target, 0.5) * (1 - (1 - target) * (1 - 2 * (blend - 0.5))) +
        (1 - step(target, 0.5)) * (target * (2 * blend));
}

float VividLight(float target, float blend)
{
    return
        step(target, 0.5) * (target / (1 - 2 * (blend - 0.5))) +
        (1 - step(target, 0.5)) * (1 - (1 - target) / (2 * blend));
}

float SpecialLight(float target, float blend)
{
    return
        step(target, 0.5) * (target / (1 - blend * 0.65)) +
        (1 - step(target, 0.5)) * (target * (2 * blend));
}

float ColorDodge(float target, float blend)
{
    return target / (1 - blend);
}

float ColorBurn(float target, float blend)
{
    return 1 - (1 - target) / blend;
}


float4 MakeBricks_Old(float4 _pixelColor, float2 _screenPos, float intensity)
{
    float2 st = _screenPos * screenSize;
    st = frac(st);
    float light = 1 - (st.x + st.y);
    light *= light;

    float4 color = float4(
        SpecialLight(_pixelColor.r, light),
        SpecialLight(_pixelColor.g, light),
        SpecialLight(_pixelColor.b, light), 1);

    return lerp(_pixelColor, light, intensity + 0.4 / pixelScale.x);
}

float4 MakeBricks(float4 _pixelColor, float2 _screenPos, float intensity)
{
    float2 st = _screenPos * screenSize;
    st = frac(st) / pixelScale;
    float light = 1 - (st.x + st.y);
    light *= light;

    float4 color = float4(
        SpecialLight(_pixelColor.r, light),
        SpecialLight(_pixelColor.g, light),
        SpecialLight(_pixelColor.b, light), 1);

    return lerp(_pixelColor, color, intensity + 0.4 / pixelScale.x);
}

float hash(float p) { p = frac(p * 0.011); p *= p + 7.5; p *= p + p; return frac(p); }
float hash(float2 p) { float3 p3 = frac(float3(p.xyx) * 0.13); p3 += dot(p3, p3.yzx + 3.333); return frac((p3.x + p3.y) * p3.z); }

// 2D Noise based on Morgan McGuire @morgan3d
// https://www.shadertoy.com/view/4dS3Wd
float noise(float2 x) {
    float2 i = floor(x);
    float2 f = frac(x);

    // Four corners in 2D of a tile
    float a = hash(i);
    float b = hash(i + float2(1.0, 0.0));
    float c = hash(i + float2(0.0, 1.0));
    float d = hash(i + float2(1.0, 1.0));

    // Simple 2D lerp using smoothstep envelope between the values.
    // return vec3(mix(mix(a, b, smoothstep(0.0, 1.0, f.x)),
    //			mix(c, d, smoothstep(0.0, 1.0, f.x)),
    //			smoothstep(0.0, 1.0, f.y)));

    // Same code, with the clamps in smoothstep and common subexpressions
    // optimized away.
    float2 u = f * f * (3.0 - 2.0 * f);
    return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
}

float fbm(float2 x) {
    int NUM_NOISE_OCTAVES = 5;

    float v = 0.0;
    float a = 0.5;
    float2 shift = float2(100, 100);
    // Rotate to reduce axial bias
    float2x2 rot = float2x2(cos(0.5), sin(0.5), -sin(0.5), cos(0.50));
    for (int i = 0; i < NUM_NOISE_OCTAVES; ++i) {
        v += a * noise(x);
        x = mul(rot, x) * 2.0 + shift;
        a *= 0.5;
    }
    return v;
}


//-----------------------------------------------------------------------------
// Vertex Shaders.
//-----------------------------------------------------------------------------

VSOutput SpriteVertexShader(
    float4 position     : POSITION0,
    float2 texCoord0 : TEXCOORD0)
{
    VSOutput output;
    output.position = mul(position, MatrixTransform);
    output.texCoord0 = texCoord0;
    return output;
}

//-----------------------------------------------------------------------------
// Pixel Shaders.
//-----------------------------------------------------------------------------

float4 PS_Shade(float4 inPosition : SV_Position, float2 uv : TEXCOORD0) : SV_TARGET0
{
    // Grab the dark version
    float blurRadius = 0.002;

    // Normal Light
    // float3 light = SAMPLE_TEXTURE(lightMap, uv).rgb;

    // Blurred Light
    float3 light = SAMPLE_TEXTURE(lightMap, uv + float2(blurRadius, 0)).rgb/4;
    light += SAMPLE_TEXTURE(lightMap, uv + float2(-blurRadius, 0)).rgb/4;
    light += SAMPLE_TEXTURE(lightMap, uv + float2(0, blurRadius)).rgb/4;
    light += SAMPLE_TEXTURE(lightMap, uv + float2(0, -blurRadius)).rgb/4;

    float shadowAmount = 1 - (light.r + light.g + light.b) / 3;

    float2 distortion = noise(uv * 100 + float2(time, time)) * float2(1,1) * pow(shadowAmount, 2);
    float3 color = tex2D(inputTexture, uv);
    

    float3 dark = tex2D(inputTexture, uv + 0.01 * (distortion - 0.5f));


    dark.g = tex2D(inputTexture, uv - 0.005 * (distortion + 0.5f)).g * 0.3;

    float lum = dot(dark.rgb, float3(0.299, 0.587, 0.114));
    dark.rgb = float3(pow(lum,2), pow(lum, 1.20), pow(lum, 0.8));

    float2 sizeFirst = 0.02;
    float2 sizeSecond = 0.008;

    dark.rgb += saturate(pow(
        fbm(uv * screenSize * sizeFirst + float2(time, time) * 0.1 + (cameraPosition)*sizeFirst) -
        fbm(uv * screenSize * sizeSecond - float2(time, time) * 0.04 + (cameraPosition)*sizeSecond), 5.2)
        * float3(0.1, 0.18, 0.35) * pow(shadowAmount, 2)) * 10;

    color = lerp(color, dark, (1- light) * shadeAmount);

    return float4(color,1);
}

float4 PS_Monitor(float4 inPosition : SV_Position, float2 uv : TEXCOORD0) : SV_TARGET0
{
    int color_depth_r = 20;
    int color_depth_g = 32;
    int color_depth_b = 27;

    int dark_color_depth_r = 4;
    int dark_color_depth_g = 7;
    int dark_color_depth_b = 12;

    // Grab the dark version
    float3 light = SAMPLE_TEXTURE(lightMap, uv).rgb;
    float shadowAmount = 1 - (light.r + light.g + light.b) / 3;

    float2 distortion = noise(uv * 100 + float2(time, time)) * float2(1,1) * pow(shadowAmount, 2);
    float3 color = tex2D(inputTexture, uv);
    float3 dark = tex2D(inputTexture, uv + 0.005 * (distortion - 0.5f));

    dark.g = tex2D(inputTexture, uv - 0.005 * (distortion + 0.5f)).g * 0.3;

    dark.rgb = float3(
        trunc(dark.r * dark_color_depth_r) / dark_color_depth_r,
        trunc(dark.g * dark_color_depth_g) / dark_color_depth_g,
        trunc(dark.b * dark_color_depth_b) / dark_color_depth_b
        );

    float lum = dot(dark.rgb, float3(0.299, 0.587, 0.114));
    dark.rgb = float3(pow(lum,2), pow(lum, 1.20), pow(lum, 0.8));

    float2 sizeFirst = 0.02;
    float2 sizeSecond = 0.008;

    dark.rgb += saturate(pow(
        fbm(uv * screenSize * sizeFirst + float2(time, time) * 0.1 + (cameraPosition)*sizeFirst) -
        fbm(uv * screenSize * sizeSecond - float2(time, time) * 0.04 + (cameraPosition)*sizeSecond), 5.2)
        * float3(0.1, 0.18, 0.35) * pow(shadowAmount, 2)) * 10;

    color = lerp(dark, color, light);

    // Convert from [0.0, 1.0] range to [0, 255] range
    float3 c = float3(round(color * 255.0));

    // Apply the dithering pattern
    float pattern = DitheringPattern(uv * screenSize);
    c += float3(pattern, pattern, pattern) * ditherAmount;

    // Truncate from 8 bits to color_depth bits
    c.rgb = float3(
        trunc(c.r * color_depth_r / 255) / color_depth_r,
        trunc(c.g * color_depth_g / 255) / color_depth_g,
        trunc(c.b * color_depth_b / 255) / color_depth_b
    );


    // Dessaturate the dakness
    lum = dot(dark.rgb, float3(0.299, 0.587, 0.114));
    c.rgb = lerp(c.rgb, float3(
        clamp(pow(lum, 2), 0.03, 1),
        clamp(pow(lum, 1.1), 0.05, 1),
        clamp(pow(lum, 0.8), 0.09, 1)),
        shadowAmount);

    c.rgb = lerp(c.rgb, c.rgb + float3(0.02, 0.07, 0.08), shadowAmount);

    c.r += lerp(saturate(pow(
        fbm(uv * screenSize * .2 * sizeFirst + float2(time, time) * 0.1 + (cameraPosition)*sizeFirst * .2) -
        fbm(uv * screenSize * 2 * sizeSecond - float2(time, time) * 0.04 + (cameraPosition)*sizeSecond * 2), 4)
        * 0.1 * pow(shadowAmount, 2)) * 150, c.r, pow(shadowAmount,0.8)) * shadowAmount;

    float4 base = MakeBricks(float4(c, 1), uv, 0.08);

    return base;
}

float4 PS_Dither(float4 inPosition : SV_Position, float2 uv : TEXCOORD0) : SV_TARGET0
{
    int color_depth_r = 20;
    int color_depth_g = 32;
    int color_depth_b = 27;

    // Grab the dark version
    float3 color = tex2D(inputTexture, uv).rgb;
    // Convert from [0.0, 1.0] range to [0, 255] range
    float3 c = float3(round(color * 255.0));

    //// Apply the dithering pattern
    float pattern = DitheringPattern(uv * screenSize);
    c += float3(pattern, pattern, pattern) * ditherAmount;

    //// Truncate from 8 bits to color_depth bits
    c.rgb = float3(
        trunc(c.r * color_depth_r / 255) / color_depth_r,
        trunc(c.g * color_depth_g / 255) / color_depth_g,
        trunc(c.b * color_depth_b / 255) / color_depth_b
    );

    //float4 base = MakeBricks(float4(color, 1), uv, 0.01);

    float4 base = float4(color, 1);
    return base;
}

//-----------------------------------------------------------------------------
// Techniques.
//-----------------------------------------------------------------------------

technique Shade
{
    pass Pass1
    {
        AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
        PixelShader = compile PS_SHADERMODEL PS_Shade();
        VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
    }
}

technique Dither
{
    pass Pass1
    {
        AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
        PixelShader = compile PS_SHADERMODEL PS_Dither();
        VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
    }
}

technique Monitor
{
    pass Pass1
    {
        AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
        PixelShader = compile PS_SHADERMODEL PS_Monitor();
        VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
    }
}
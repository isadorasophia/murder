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
DECLARE_TEXTURE(gradeFrom, 1)
{
    AddressU = Clamp;
    AddressV = Clamp;
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
};
DECLARE_TEXTURE(gradeTo, 2)
{
    AddressU = Clamp;
    AddressV = Clamp;
    MinFilter = Point;
    MagFilter = Point;
    MipFilter = Point;
};
float percent;
int colorGradeSize;


// THis goes out of your Vertex Shader into your Pixel Shader
struct VSOutput
{
    float4 position     : SV_Position;
    float2 texCoord0    : TEXCOORD0;
};


//-----------------------------------------------------------------------------
// Vertex Shaders.
//-----------------------------------------------------------------------------

VSOutput SpriteVertexShader(
    float4 position     : POSITION0,
    float2 texCoord0 : TEXCOORD0)
{
    VSOutput output;
    output.position = mul(position, MatrixTransform);
    output.texCoord0 = texCoord0;;
    return output;
}

//-----------------------------------------------------------------------------
// Pixel Shaders.
//-----------------------------------------------------------------------------

// lerps between 2 color grades (gradeFrom, gradeTo, by percent)
float4 PS_ColorGradeFromTo(float4 inPosition : SV_Position, float2 uv : TEXCOORD0) : SV_TARGET0
{
    float4 color = tex2D(inputTexture, uv);

    float size = colorGradeSize;
    float sqrd = size * size;

    float offX = color.x * (1.0 / sqrd) * (size - 1.0) + (1.0 / sqrd) * 0.5;
    float offY = color.y + (1.0 / size) * 0.5;
    float zSlice0 = min(floor(color.z * size), size - 1.0);
    float zSlice1 = min(zSlice0 + 1.0, size - 1.0);

    float2 index0 = float2(offX + zSlice0 / size, offY);
    float2 index1 = float2(offX + zSlice1 / size, offY);
    float3 from0 = SAMPLE_TEXTURE(gradeFrom, index0).xyz;
    float3 from1 = SAMPLE_TEXTURE(gradeFrom, index1).xyz;
    float3 to0 = SAMPLE_TEXTURE(gradeTo, index0).xyz;
    float3 to1 = SAMPLE_TEXTURE(gradeTo, index1).xyz;

    float zOffset = fmod(color.z * size, 1.0);
    float3 from = lerp(from0, from1, zOffset);
    float3 to = lerp(to0, to1, zOffset);

    return float4(lerp(from, to, percent) * color.a, color.a);
}

// samples from a single color grade (gradeFrom)
float4 PS_ColorGrade(float4 inPosition : SV_Position, float2 uv : TEXCOORD0) : SV_TARGET0
{
    float4 color = tex2D(inputTexture, uv);

    float size = colorGradeSize;
    float sqrd = size * size;

    float offX = color.x * (1.0 / sqrd) * (size - 1.0) + (1.0 / sqrd) * 0.5;
    float offY = color.y + (1.0 / size) * 0.5;
    float zSlice0 = min(floor(color.z * size), size - 1.0);
    float zSlice1 = min(zSlice0 + 1.0, size - 1.0);

    float3 sample0 = SAMPLE_TEXTURE(gradeFrom, float2(offX + zSlice0 / size, offY)).xyz;
    float3 sample1 = SAMPLE_TEXTURE(gradeFrom, float2(offX + zSlice1 / size, offY)).xyz;

    return float4(lerp(sample0, sample1, fmod(color.z * size, 1.0)) * color.a, color.a);
}

// samples from a single color grade (gradeFrom)
float4 PS_ColorGradeSnap(float4 inPosition : SV_Position, float2 uv : TEXCOORD0) : SV_TARGET0
{
    float4 color = tex2D(inputTexture, uv);

    float size = colorGradeSize;
    float sqrd = size * size;

    float offX = color.x * (1.0 / sqrd) * (size - 1.0) + (1.0 / sqrd) * 0.5;
    float offY = color.y + (1.0 / size) * 0.5;
    float zSlice0 = min(floor(color.z * size), size - 1.0);
    float zSlice1 = min(zSlice0 + 1.0, size - 1.0);

    float3 sample0 = SAMPLE_TEXTURE(gradeFrom, float2(offX + zSlice0 / size, offY)).xyz;
    float3 sample1 = SAMPLE_TEXTURE(gradeFrom, float2(offX + zSlice1 / size, offY)).xyz;

    return float4(sample0 * color.a, color.a);
}

//-----------------------------------------------------------------------------
// Techniques.
//-----------------------------------------------------------------------------

technique ColorGradeSingle
{
    pass Pass1
    {
        AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
        PixelShader = compile PS_SHADERMODEL PS_ColorGrade();
        VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
    }
}


technique ColorGradeFromTo
{
    pass Pass1
    {
        AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
        PixelShader = compile PS_SHADERMODEL PS_ColorGradeFromTo();
        VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
    }
}

technique ColorGradeSnap
{
    pass Pass1
    {
        AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
        PixelShader = compile PS_SHADERMODEL PS_ColorGradeSnap();
        VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
    }
}
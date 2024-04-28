//-----------------------------------------------------------------------------
// Defines
//-----------------------------------------------------------------------------

#ifdef SM4 // shader model 4.0 (DX11)

    #define VS_SHADERMODEL vs_4_0
    #define PS_SHADERMODEL ps_4_0

    #define DECLARE_TEXTURE(Name, index) \
        Texture2D<float4> Name : register(t##index); \
        sampler Name##Sampler : register(s##index)

    #define DECLARE_TEXTURE_CLAMP(Name, index) \
        Texture2D<float4> Name : register(t##index); \
        sampler Name##Sampler : register(s##index) = sampler_state { AddressU = Clamp; AddressV = Clamp; Texture = (Name); }

    #define DECLARE_TEXTURE_WRAP(Name, index) \
        Texture2D<float4> Name : register(t##index); \
        sampler Name##Sampler : register(s##index) = sampler_state { Filter = MIN_MAG_MIP_POINT; AddressU = Wrap; AddressV = Wrap; Texture = (Name); }

    #define SAMPLE_TEXTURE(Name, texCoord)  Name.Sample(Name##Sampler, texCoord)

#else // shader model 2.0 (DX9)

    #define VS_SHADERMODEL vs_2_0
    #define PS_SHADERMODEL ps_3_0

    #define DECLARE_TEXTURE(Name, index) \
        texture2D Name; \
        sampler Name##Sampler : register(s##index) = sampler_state { Texture = (Name); }

    #define DECLARE_TEXTURE_CLAMP(Name, index) \
        Texture2D<float4> Name : register(t##index); \
        sampler Name##Sampler : register(s##index) = sampler_state { AddressU = Clamp; AddressV = Clamp; Texture = (Name); }

    #define DECLARE_TEXTURE_WRAP(Name, index) \
        Texture2D<float4> Name : register(t##index); \
        sampler Name##Sampler : register(s##index) = sampler_state { Filter = MIN_MAG_MIP_POINT; AddressU = Wrap; AddressV = Wrap; Texture = (Name); }

    #define SAMPLE_TEXTURE(Name, texCoord)  tex2D(Name##Sampler, texCoord)

#endif

//-----------------------------------------------------------------------------
// Default parameters
//-----------------------------------------------------------------------------
DECLARE_TEXTURE(Texture, 0);
float4x4 MatrixTransform;


//-----------------------------------------------------------------------------
// Default implementations
//-----------------------------------------------------------------------------
struct VSOutput
{
	float4 position     : SV_Position;
	float4 color        : COLOR0;
	float2 uv           : TEXCOORD0;
    float3 blend        : TEXCOORD1;
};

VSOutput DefaultVertexShader(
	float4 position     : POSITION0,
	float4 color		: COLOR0,
	float2 texCoord0	: TEXCOORD0,
	float3 texCoord1	: TEXCOORD1)
{
	VSOutput output;
	output.position = mul(position, MatrixTransform);
	output.color    = color;
	output.uv       = texCoord0;
	output.blend    = texCoord1;
	return output;
}

// Most shaders just have a single pass technique
#define SINGLE_PASS_TECHNIQUE \
    technique DefaultTechnique { \
        pass Pass1 \
        { \
            PixelShader = compile PS_SHADERMODEL MainPixelShader(); \
            VertexShader = compile VS_SHADERMODEL DefaultVertexShader(); \
        } \
    };

#define TECHNIQUE(techniqueName, pixelShader) \
    technique techniqueName { \
        pass Pass1 \
        { \
            PixelShader = compile PS_SHADERMODEL pixelShader(); \
            VertexShader = compile VS_SHADERMODEL DefaultVertexShader(); \
        } \
    }
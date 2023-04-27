#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif

float4x4 MatrixTransform;
sampler inputTexture;
float levels = 1;
float aberrationStrength = 0.01;

// THis goes out of your Vertex Shader into your Pixel Shader
struct VSOutput
{
	float4 position     : SV_Position;
	float2 texCoord0    : TEXCOORD0;
	float4 color		: COLOR0;
};

VSOutput SpriteVertexShader(
	float4 position     : POSITION0,
	float2 texCoord0	: TEXCOORD0,
	float4 color		: COLOR0)
{
	VSOutput output;
	output.position = mul(position, MatrixTransform);
	output.texCoord0 = texCoord0;
	output.color = color;
	return output;
}


float4 Posterize(float4 color, float levels)
{
    float min = 1 / levels;
    return floor(color * levels) / (levels - 1.0) - float4(min, min, min, min);
}

float4 PosterizeWithBlack(float4 color, float levels)
{
    float4 adjustedColor = (color * (levels - 1.0) + 0.5) / levels;
    return floor(adjustedColor) * (levels / (levels - 1.0));
}

float2 ComputeAberration(float2 uv, float2 center, float strength)
{
    float2 direction = uv - center;
    float distance = length(direction);
    return direction * distance * strength;
}

float4 SimpleSpritePixelShader(VSOutput input) : SV_Target0
{
    float2 center = float2(0.5, 0.5);
    
    float2 redOffset = ComputeAberration(input.texCoord0, center, aberrationStrength);
    float2 greenOffset = ComputeAberration(input.texCoord0, center, 0);
    float2 blueOffset = ComputeAberration(input.texCoord0, center, -aberrationStrength);

    float red = tex2D(inputTexture, input.texCoord0 + redOffset).r;
    float green = tex2D(inputTexture, input.texCoord0 + greenOffset).g;
    float blue = tex2D(inputTexture, input.texCoord0 + blueOffset).b;
    float alpha = tex2D(inputTexture, input.texCoord0).a;
    
    float4 color = float4(red, green, blue, alpha);
    color = Posterize(color, levels);
    return color;
}


technique Simple
{
	pass Pass1
	{
		PixelShader = compile PS_SHADERMODEL SimpleSpritePixelShader();
		VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
	}
}
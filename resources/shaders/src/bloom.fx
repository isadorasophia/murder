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

float bloomThreshold = 0.95;
float sWidth = 640;
float sHeight = 320;


// THis goes out of your Vertex Shader into your Pixel Shader
struct VSOutput
{
	float4 position     : SV_Position;
	float2 texCoord0    : TEXCOORD0;
	float4 color		: COLOR0;
};

VSOutput SpriteVertexShader(
	float4 position     : POSITION0,
	float2 texCoord0 : TEXCOORD0,
	float4 color : COLOR0)
{
	VSOutput output;
	output.position = mul(position, MatrixTransform);
	output.texCoord0 = texCoord0;
	output.color = color;
	return output;
}

float4 BrightPass(VSOutput input) : SV_Target0
{
    float4 color = tex2D(inputTexture, input.texCoord0);
    float brightness = max(color.r, max(color.g, color.b));
    float4 result = (brightness > bloomThreshold) ? color : float4(0, 0, 0, 0);
    return result;
}

float4 GaussianBlur(VSOutput input) : SV_Target0
{
    float2 texelSize = float2(1.0 / sWidth, 1.0 / sHeight);
    float4 sum = float4(0, 0, 0, 0);
    float weightSum = 0.0;

    for (int x = -2; x <= 2; ++x)
    {
        for (int y = -2; y <= 2; ++y)
        {
            float2 offset = float2(x, y) * texelSize;
            float weight = exp(-0.5 * (x * x + y * y));
            sum += tex2D(inputTexture, input.texCoord0 + offset) * weight;
            weightSum += weight;
        }
    }

    return sum / weightSum;
}

technique Bloom_BrightPass
{
    pass BrightPass
    {
        PixelShader = compile PS_SHADERMODEL BrightPass();
        VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
    }
}


technique Bloom_GaussianBlurHorizontal
{
    pass GaussianBlurHorizontal
    {
        PixelShader = compile PS_SHADERMODEL GaussianBlur();
        VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
    }
}

technique Bloom_GaussianBlurVertical
{
    pass GaussianBlurVertical
    {
        PixelShader = compile PS_SHADERMODEL GaussianBlur();
        VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
    }
};
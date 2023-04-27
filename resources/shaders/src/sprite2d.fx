#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#endif

#define mod(x,y) (x-y*floor(x/y))

float inputTime;
float4x4 MatrixTransform;
sampler inputTexture;

// THis goes out of your Vertex Shader into your Pixel Shader
struct VSOutput
{
	float4 position     : SV_Position;
	float4 color        : COLOR0;
	float2 texCoord0    : TEXCOORD0;
	float3 texCoord1    : TEXCOORD1;
};

VSOutput SpriteVertexShader(
	float4 position     : POSITION0,
	float4 color		: COLOR0,
	float2 texCoord0	: TEXCOORD0,
	float3 texCoord1	: TEXCOORD1)
{
	VSOutput output;
	output.position = mul(position, MatrixTransform);
	output.color = color;
	output.texCoord0 = texCoord0;
	output.texCoord1 = texCoord1;
	return output;
}

float4 SpritePixelShader(VSOutput input) : SV_Target0
{
	float4 color = tex2D(inputTexture, input.texCoord0);
	return
		input.texCoord1.x * color * input.color +														  // Basic		   //
		input.texCoord1.y * float4(lerp(color.rgb, input.color.rgb, input.color.a) * color.a, color.a) + //  Wash		  //
		input.texCoord1.z * input.color;																//   Color only  //
}

float4 DiagonalLinesPixelShader(VSOutput input) : SV_Target0
{
	// Normalized pixel coordinates (from 0 to 1)

	float fxAlpha =  smoothstep(0.15, 0.85, 0.5 + sin(inputTime * 10 + (input.position.x + input.position.y) * 1)/2.);
	
	float4 color = tex2D(inputTexture, input.texCoord0);
	return color * float4(input.color.rgb, input.color.a * fxAlpha);								// Basic		  // (This techinique doesn't need wash and color only)
}

technique Alpha
{
	pass Pass1
	{
		PixelShader = compile PS_SHADERMODEL SpritePixelShader();
		VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
	}
};

technique DiagonalLines
{
	pass Pass1
	{
		PixelShader = compile PS_SHADERMODEL DiagonalLinesPixelShader();
		VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
	}
};
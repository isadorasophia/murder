
// Vertex Shader Input

struct VSIn_PosColor {
	float4 Position             : POSITION0;
	float4 Color                : COLOR0;
    float4 PositionPS           : TEXCOORD0;
};

struct VSIn_PosColorTexture {
    float4 Position             : POSITION0;
    float4 Color                : COLOR0;
    float2 TextureCoord         : TEXCOORD0;
    float4 PositionPS           : TEXCOORD1;
};

/*
struct VSIn_PosColorTextureDepth {
    float4 Position             : POSITION0;
    float4 Color                : COLOR0;
    float2 TextureCoord         : TEXCOORD0;
    float  Depth                : TEXCOORD1;
    float4 PositionPS           : TEXCOORD2;
};
*/

// Vertex Shader Output

struct VSOut_PosColor {
	float4 Position             : POSITION0;
	float4 Diffuse              : COLOR0;
    float4 PositionPS           : TEXCOORD0;
};

struct VSOut_PosColorTexture {
    float4 Position             : POSITION0;
    float4 Diffuse              : COLOR0;
    float2 TextureCoord         : TEXCOORD0;
    float4 PositionPS           : TEXCOORD1;
};

struct VSOut_PosColorDepth {
	float4 Position             : POSITION0;
	float4 Diffuse              : COLOR0;
    float  Depth                : TEXCOORD0;
    float4 PositionPS           : TEXCOORD1;
};

struct VSOut_PosColorTextureDepth {
    float4 Position             : POSITION0;
    float4 Diffuse              : COLOR0;
    float2 TextureCoord         : TEXCOORD0;
    float  Depth                : TEXCOORD1;
    float4 PositionPS           : TEXCOORD2;
};

// Pixel Shader Input

struct PSIn_PosColor {
	float4 Position             : TEXCOORD0;
	float4 Diffuse              : COLOR0;
};

struct PSIn_PosColorTexture {
    float4 Diffuse              : COLOR0;
    float2 TextureCoord         : TEXCOORD0;
    float4 Position             : TEXCOORD1;
};

struct PSIn_PosColorDepth {
    float4 Diffuse              : COLOR0;
    float  Depth                : TEXCOORD0;
    float4 Position             : TEXCOORD1;
};

struct PSIn_PosColorTextureDepth {
    float4 Diffuse              : COLOR0;
    float2 TextureCoord         : TEXCOORD0;
    float  Depth                : TEXCOORD1;
    float4 Position             : TEXCOORD2;
};

// Pixel Shader Output

struct PSOut_ColorDepth {
    float4 Color : COLOR0;
    float  Depth : DEPTH;
};
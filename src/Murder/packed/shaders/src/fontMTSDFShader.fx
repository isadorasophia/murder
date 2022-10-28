#include "Structures.fxh"
#include "Helper.fxh"

Texture2D Texture;
sampler2D TextureSampler = sampler_state {
    Texture = <Texture>;
    MipFilter = Linear;
    MinFilter = Linear;
    MagFilter = Linear;
    AddressU = Clamp;
    AddressV = Clamp;
};

float4x4 WorldViewProj;
float4 DiffuseColor;

float ScreenPixelRange;

float median(float r, float g, float b) {
    return max(min(r, g), min(max(r, g), b));
}

// Vertex Color, Texture, Depth

VSOut_PosColorTextureDepth VSVertexColorTextureDepth(VSIn_PosColorTexture input) {
    VSOut_PosColorTextureDepth output;

    output.Position = mul(float4(input.Position.x, input.Position.y, 0.0f, 1.0f), WorldViewProj);
    output.PositionPS = input.Position;
    output.Depth = input.Position.z / input.Position.w;
    output.Diffuse = DiffuseColor * input.Color;
    output.TextureCoord = input.TextureCoord;

    return output;
}

PSOut_ColorDepth PSVertexColorTextureDepth(PSIn_PosColorTextureDepth input) {
    PSOut_ColorDepth psOut;

    float4 px = tex2D(TextureSampler, input.TextureCoord);
    float screenPxDistance = ScreenPixelRange * (median(px.r, px.g, px.b) - 0.5);
    float alpha = clamp(screenPxDistance + 0.5, 0.0, 1.0);

    psOut.Color = float4(input.Diffuse.rgb, 1.0f) * input.Diffuse.a * alpha;

    if (psOut.Color.a < .05f) {
        discard;
    }

    psOut.Depth = input.Depth;
    return psOut;
}

// Techniques

technique Basic_PosColor_Texture_Depth {
    pass {
        VertexShader = compile vs_3_0 VSVertexColorTextureDepth();
        PixelShader = compile ps_3_0 PSVertexColorTextureDepth();
    }
};
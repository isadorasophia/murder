#include "murder.fxh"

float4 MainPixelShader(VSOutput input) : SV_Target0
{
	float4 color = SAMPLE_TEXTURE(Texture, input.uv);

	return color * input.color;
}

SINGLE_PASS_TECHNIQUE
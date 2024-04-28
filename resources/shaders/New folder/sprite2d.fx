#include "murder.fxh"

float inputTime;

float4 MainPixelShader(VSOutput input) : SV_Target0
{
	float4 color = SAMPLE_TEXTURE(Texture, input.uv);
	return
		input.blend.x * color * input.color +														  // Basic		   //
		input.blend.y * float4(lerp(color.rgb, input.color.rgb, input.color.a) * color.a, color.a) + //  Wash		  //
		input.blend.z * input.color;																//   Color only  //
}

float4 DiagonalLinesPixelShader(VSOutput input) : SV_Target0
{
	// Normalized pixel coordinates (from 0 to 1)

	float fxAlpha =  smoothstep(0.15, 0.85, 0.5 + sin(inputTime * 10 + (input.position.x + input.position.y) * 1)/2.);
	
	float4 color = SAMPLE_TEXTURE(Texture, input.uv);
	return color * float4(input.color.rgb, input.color.a * fxAlpha);								// Basic		  // (This techinique doesn't need wash and color only)
}

TECHNIQUE(Alpha, MainPixelShader);
TECHNIQUE(DiagonalLines, DiagonalLinesPixelShader);
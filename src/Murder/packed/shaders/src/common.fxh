//-----------------------------------------------------------------------------
// Defines (so this compiles for PS4 or XNA)
//-----------------------------------------------------------------------------

// PS4
#if __PSSL__

#define DECLARE_TEXTURE(name, index) \
    Texture2D name : register(t##index); \
    SamplerState name##Sampler : register(s##index)

#define SAMPLE_TEXTURE(Name, texCoord) Name.Sample(Name##Sampler, texCoord)

#define PS_3_SHADER_COMPILER sce_ps_orbis
#define PS_2_SHADER_COMPILER sce_ps_orbis
#define VS_SHADER_COMPILER sce_vs_vs_orbis

#define SV_TARGET0 S_TARGET_OUTPUT0
#define SV_TARGET1 S_TARGET_OUTPUT1
#define SV_TARGET2 S_TARGET_OUTPUT2
#define SV_Position S_POSITION

#elif XBOXONE

#define DECLARE_TEXTURE(name, index) \
    Texture2D<float4> name : register(t##index); \
    sampler name##__XBSAMP : register(s##index)

#define SAMPLE_TEXTURE(name, texCoord) name.Sample(name##__XBSAMP, texCoord)

#define PS_3_SHADER_COMPILER ps_5_0
#define PS_2_SHADER_COMPILER ps_5_0
#define VS_SHADER_COMPILER vs_5_0


// XNA / DirectX 11
#else

#define DECLARE_TEXTURE(Name, index) \
    texture Name: register(t##index); \
    sampler Name##Sampler: register(s##index)

#define SAMPLE_TEXTURE(Name, texCoord) tex2D(Name##Sampler, texCoord)

#define PS_3_SHADER_COMPILER ps_3_0
#define PS_2_SHADER_COMPILER ps_2_0
#define VS_SHADER_COMPILER vs_3_0
#define VS_2_SHADER_COMPILER vs_2_0

#define SV_TARGET0 COLOR0
#define SV_TARGET1 COLOR1
#define SV_TARGET2 COLOR2

#endif
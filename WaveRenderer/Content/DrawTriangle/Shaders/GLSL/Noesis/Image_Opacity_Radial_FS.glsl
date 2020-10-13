#version 450

layout(binding = 2, std140) uniform type_Buffer0
{
    vec4 radialGrad[2];
    float opacity;
} Buffer0;

uniform sampler2D SPIRV_Cross_CombinedrampsrampsSampler;
uniform sampler2D SPIRV_Cross_CombinedimageimageSampler;

layout(location = 0) in vec2 in_var_TEXCOORD0;
layout(location = 1) in vec2 in_var_TEXCOORD1;
layout(location = 0) out vec4 out_var_SV_Target0;

void main()
{
    float _47 = (Buffer0.radialGrad[1].y * in_var_TEXCOORD0.x) - (Buffer0.radialGrad[1].z * in_var_TEXCOORD0.y);
    out_var_SV_Target0 = texture(SPIRV_Cross_CombinedimageimageSampler, in_var_TEXCOORD1) * (Buffer0.opacity * texture(SPIRV_Cross_CombinedrampsrampsSampler, vec2(((Buffer0.radialGrad[0].x * in_var_TEXCOORD0.x) + (Buffer0.radialGrad[0].y * in_var_TEXCOORD0.y)) + (Buffer0.radialGrad[0].z * sqrt((((Buffer0.radialGrad[0].w * in_var_TEXCOORD0.x) * in_var_TEXCOORD0.x) + ((Buffer0.radialGrad[1].x * in_var_TEXCOORD0.y) * in_var_TEXCOORD0.y)) - (_47 * _47))), Buffer0.radialGrad[1].w)).w);
}


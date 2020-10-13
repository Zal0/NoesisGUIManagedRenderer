#version 450

layout(binding = 2, std140) uniform type_Buffer0
{
    float opacity;
} Buffer0;

uniform sampler2D SPIRV_Cross_CombinedpatternpatternSampler;
uniform sampler2D SPIRV_Cross_CombinedglyphsglyphsSampler;

layout(location = 0) in vec2 in_var_TEXCOORD0;
layout(location = 1) in vec2 in_var_TEXCOORD1;
layout(location = 2) in vec2 in_var_TEXCOORD4;
layout(location = 0) out vec4 out_var_SV_Target0;

void main()
{
    float _57 = length(dFdx(in_var_TEXCOORD4));
    float _63 = (-0.64999997615814208984375) * (1.0 - ((clamp(1.0 / _57, 0.125, 0.25) - 0.125) * 8.0));
    float _64 = 0.64999997615814208984375 * _57;
    out_var_SV_Target0 = texture(SPIRV_Cross_CombinedpatternpatternSampler, in_var_TEXCOORD0) * (smoothstep(_63 - _64, _63 + _64, 7.96875 * (texture(SPIRV_Cross_CombinedglyphsglyphsSampler, in_var_TEXCOORD1).x - 0.501960813999176025390625)) * Buffer0.opacity);
}


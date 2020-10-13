#version 450

layout(binding = 2, std140) uniform type_Buffer0
{
    float opacity;
} Buffer0;

layout(binding = 1, std140) uniform type_Buffer1
{
    vec4 textureSize;
} Buffer1;

uniform sampler2D SPIRV_Cross_CombinedrampsrampsSampler;
uniform sampler2D SPIRV_Cross_CombinedglyphsglyphsSampler;

layout(location = 0) in vec2 in_var_TEXCOORD0;
layout(location = 1) in vec2 in_var_TEXCOORD1;
layout(location = 2) in vec2 in_var_TEXCOORD4;
layout(location = 0) out vec4 out_var_SV_Target0;
layout(location = 1) out vec4 out_var_SV_Target1;

void main()
{
    vec4 _54 = texture(SPIRV_Cross_CombinedrampsrampsSampler, in_var_TEXCOORD0);
    vec2 _57 = dFdx(in_var_TEXCOORD4);
    vec2 _62 = (_57 * Buffer1.textureSize.zw) * 0.3333333432674407958984375;
    float _79 = length(_57);
    float _85 = (-0.64999997615814208984375) * (1.0 - ((clamp(1.0 / _79, 0.125, 0.25) - 0.125) * 8.0));
    float _86 = 0.64999997615814208984375 * _79;
    vec3 _91 = smoothstep(vec3(_85 - _86), vec3(_85 + _86), (vec3(texture(SPIRV_Cross_CombinedglyphsglyphsSampler, in_var_TEXCOORD1 - _62).x, texture(SPIRV_Cross_CombinedglyphsglyphsSampler, in_var_TEXCOORD1).x, texture(SPIRV_Cross_CombinedglyphsglyphsSampler, in_var_TEXCOORD1 + _62).x) - vec3(0.501960813999176025390625)) * 7.96875);
    float _95 = _91.y;
    out_var_SV_Target0 = vec4((_54.xyz * Buffer0.opacity) * _91, _95);
    out_var_SV_Target1 = vec4(_91 * (Buffer0.opacity * _54.w), _95);
}


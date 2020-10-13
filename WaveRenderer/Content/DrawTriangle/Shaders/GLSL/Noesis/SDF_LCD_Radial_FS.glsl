#version 450

layout(binding = 2, std140) uniform type_Buffer0
{
    vec4 radialGrad[2];
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
    float _65 = (Buffer0.radialGrad[1].y * in_var_TEXCOORD0.x) - (Buffer0.radialGrad[1].z * in_var_TEXCOORD0.y);
    vec4 _95 = texture(SPIRV_Cross_CombinedrampsrampsSampler, vec2(((Buffer0.radialGrad[0].x * in_var_TEXCOORD0.x) + (Buffer0.radialGrad[0].y * in_var_TEXCOORD0.y)) + (Buffer0.radialGrad[0].z * sqrt((((Buffer0.radialGrad[0].w * in_var_TEXCOORD0.x) * in_var_TEXCOORD0.x) + ((Buffer0.radialGrad[1].x * in_var_TEXCOORD0.y) * in_var_TEXCOORD0.y)) - (_65 * _65))), Buffer0.radialGrad[1].w));
    vec2 _98 = dFdx(in_var_TEXCOORD4);
    vec2 _103 = (_98 * Buffer1.textureSize.zw) * 0.3333333432674407958984375;
    float _120 = length(_98);
    float _126 = (-0.64999997615814208984375) * (1.0 - ((clamp(1.0 / _120, 0.125, 0.25) - 0.125) * 8.0));
    float _127 = 0.64999997615814208984375 * _120;
    vec3 _132 = smoothstep(vec3(_126 - _127), vec3(_126 + _127), (vec3(texture(SPIRV_Cross_CombinedglyphsglyphsSampler, in_var_TEXCOORD1 - _103).x, texture(SPIRV_Cross_CombinedglyphsglyphsSampler, in_var_TEXCOORD1).x, texture(SPIRV_Cross_CombinedglyphsglyphsSampler, in_var_TEXCOORD1 + _103).x) - vec3(0.501960813999176025390625)) * 7.96875);
    float _136 = _132.y;
    out_var_SV_Target0 = vec4((_95.xyz * Buffer0.opacity) * _132, _136);
    out_var_SV_Target1 = vec4(_132 * (Buffer0.opacity * _95.w), _136);
}


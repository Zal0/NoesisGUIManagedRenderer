#version 450

uniform sampler2D SPIRV_Cross_CombinedglyphsglyphsSampler;

layout(location = 0) in vec4 in_var_COLOR;
layout(location = 1) in vec2 in_var_TEXCOORD1;
layout(location = 2) in vec2 in_var_TEXCOORD4;
layout(location = 0) out vec4 out_var_SV_Target0;

void main()
{
    float _43 = length(dFdx(in_var_TEXCOORD4));
    float _49 = (-0.64999997615814208984375) * (1.0 - ((clamp(1.0 / _43, 0.125, 0.25) - 0.125) * 8.0));
    float _50 = 0.64999997615814208984375 * _43;
    out_var_SV_Target0 = in_var_COLOR * smoothstep(_49 - _50, _49 + _50, 7.96875 * (texture(SPIRV_Cross_CombinedglyphsglyphsSampler, in_var_TEXCOORD1).x - 0.501960813999176025390625));
}


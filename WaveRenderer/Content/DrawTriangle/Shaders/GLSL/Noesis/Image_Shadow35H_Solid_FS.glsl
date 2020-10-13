#version 450

uniform sampler2D SPIRV_Cross_CombinedimageimageSampler;

layout(location = 0) in vec4 in_var_COLOR;
layout(location = 1) in vec2 in_var_TEXCOORD1;
layout(location = 2) in vec4 in_var_TEXCOORD2;
layout(location = 0) out vec4 out_var_SV_Target0;

void main()
{
    vec4 _34 = texture(SPIRV_Cross_CombinedimageimageSampler, clamp(in_var_TEXCOORD1, in_var_TEXCOORD2.xy, in_var_TEXCOORD2.zw));
    out_var_SV_Target0 = (_34 + vec4(1.0 - _34.w)) * in_var_COLOR.w;
}


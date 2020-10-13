#version 450

uniform sampler2D SPIRV_Cross_CombinedimageimageSampler;

layout(location = 0) in vec4 in_var_COLOR;
layout(location = 1) in vec2 in_var_TEXCOORD1;
layout(location = 0) out vec4 out_var_SV_Target0;

void main()
{
    out_var_SV_Target0 = texture(SPIRV_Cross_CombinedimageimageSampler, in_var_TEXCOORD1) * in_var_COLOR.w;
}


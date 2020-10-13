#version 450

layout(location = 0) in vec4 in_var_COLOR;
layout(location = 1) in float in_var_TEXCOORD3;
layout(location = 0) out vec4 out_var_SV_Target0;

void main()
{
    out_var_SV_Target0 = in_var_COLOR * in_var_TEXCOORD3;
}


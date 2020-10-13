#version 450

layout(binding = 0, std140) uniform type_Buffer0
{
    mat4 projectionMtx;
} Buffer0;

layout(location = 0) in vec2 in_var_POSITION;
layout(location = 1) in vec2 in_var_TEXCOORD0;
layout(location = 2) in float in_var_TEXCOORD3;
layout(location = 0) out vec2 out_var_TEXCOORD0;
layout(location = 1) out float out_var_TEXCOORD3;

void main()
{
    gl_Position = Buffer0.projectionMtx * vec4(in_var_POSITION, 0.0, 1.0);
    out_var_TEXCOORD0 = in_var_TEXCOORD0;
    out_var_TEXCOORD3 = in_var_TEXCOORD3;
}


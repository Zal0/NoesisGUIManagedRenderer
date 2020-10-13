#version 450

layout(binding = 0, std140) uniform type_Buffer0
{
    mat4 projectionMtx;
} Buffer0;

layout(binding = 1, std140) uniform type_Buffer1
{
    vec2 textureSize;
} Buffer1;

layout(location = 0) in vec2 in_var_POSITION;
layout(location = 1) in vec2 in_var_TEXCOORD0;
layout(location = 2) in vec2 in_var_TEXCOORD1;
layout(location = 0) out vec2 out_var_TEXCOORD0;
layout(location = 1) out vec2 out_var_TEXCOORD1;
layout(location = 2) out vec2 out_var_TEXCOORD4;

void main()
{
    gl_Position = Buffer0.projectionMtx * vec4(in_var_POSITION, 0.0, 1.0);
    out_var_TEXCOORD0 = in_var_TEXCOORD0;
    out_var_TEXCOORD1 = in_var_TEXCOORD1;
    out_var_TEXCOORD4 = in_var_TEXCOORD1 * Buffer1.textureSize;
}


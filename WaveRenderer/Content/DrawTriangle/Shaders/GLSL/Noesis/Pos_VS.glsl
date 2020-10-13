#version 450

layout(binding = 0, std140) uniform type_Buffer0
{
    mat4 projectionMtx;
} Buffer0;

layout(location = 0) in vec2 in_var_POSITION;

void main()
{
    gl_Position = Buffer0.projectionMtx * vec4(in_var_POSITION, 0.0, 1.0);
}


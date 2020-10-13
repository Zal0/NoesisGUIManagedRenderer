#version 450

layout(binding = 2, std140) uniform type_Buffer0
{
    vec4 rgba;
} Buffer0;

layout(location = 0) out vec4 out_var_SV_Target0;

void main()
{
    out_var_SV_Target0 = Buffer0.rgba;
}


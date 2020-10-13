#version 450

void main()
{
    vec4 _31;
    if (uint(gl_VertexID) == 0u)
    {
        _31 = vec4(-1.0, 1.0, 1.0, 1.0);
    }
    else
    {
        _31 = mix(vec4(-1.0, -3.0, 1.0, 1.0), vec4(3.0, 1.0, 1.0, 1.0), bvec4(uint(gl_VertexID) == 1u));
    }
    gl_Position = _31;
}


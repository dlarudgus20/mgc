#version 330 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec3 color;

uniform mat4 vmMatrix;

out VS_OUT
{
    vec4 pos;
    vec4 color;
} vs_out;

void main()
{
    vs_out.color = vec4(color, 1.0);
    gl_Position = vmMatrix * vec4(position, 1.0);
    vs_out.pos = gl_Position;
}

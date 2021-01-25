#version 330 core

in VS_OUT
{
    vec4 pos;
    vec4 color;
} vs_out;

out vec4 fragColor;

void main()
{
    fragColor = vs_out.color;
}

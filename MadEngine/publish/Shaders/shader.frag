#version 330 core
out vec4 FragColor;

in vec2 texCoord;

uniform sampler2D texture0;
uniform vec4 uColor;

void main()
{
    FragColor = texture(texture0, texCoord) * uColor;
}
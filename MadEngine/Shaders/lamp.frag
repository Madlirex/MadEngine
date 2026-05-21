#version 330 core
out vec4 FragColor;

in vec2 texCoord;

uniform sampler2D texture0;
uniform vec4 uColor;
uniform int useTexture;

void main()
{
    if (useTexture == 1)
        FragColor = texture(texture0, texCoord) * uColor;
    else
        FragColor = uColor;
}
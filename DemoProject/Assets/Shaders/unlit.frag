#version 330 core

struct Material {
    vec4 ambientColor;
    vec4 diffuseColor;
    vec4 specularColor;

    sampler2D diffuse;
    int useDiffuse;
    sampler2D specular;
    int useSpecular;

    float shininess;
};

out vec4 FragColor;

in vec2 texCoord;

uniform sampler2D texture0;
uniform Material material;
uniform int useTexture;

void main()
{
    if (material.useDiffuse == 1)
        FragColor = texture(material.diffuse, texCoord) * material.ambientColor;
    else
        FragColor = material.ambientColor;
}
#version 330 core
out vec4 FragColor;

in vec2 texCoord;
in vec3 normal;
in vec3 FragPos;

uniform sampler2D texture0;
uniform vec4 uColor;
uniform vec4 lightColor;
uniform vec3 lightPos;

uniform vec3 viewPos;

uniform int useTexture;

void main()
{
    float ambientStrength = 0.1;
    vec4 ambient = lightColor * ambientStrength;
    
    vec3 norm = normalize(normal);
    vec3 lightDir = normalize(lightPos - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    vec4 diffuse = diff * lightColor;

    float specularStrength = 0.5;
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);

    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);
    vec4 specular = specularStrength * spec * lightColor;

    vec4 objectColor;
    
    if (useTexture == 1)
        objectColor = texture(texture0, texCoord) * uColor;
    else
        objectColor = uColor;
    
    FragColor = (ambient + diffuse + specular) * objectColor;
}
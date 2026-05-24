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

struct Light {
    vec3 position;
    
    vec4 ambient;
    vec4 diffuse;
    vec4 specular;
};

out vec4 FragColor;

in vec2 texCoord;
in vec3 normal;
in vec3 FragPos;

uniform vec3 viewPos;

uniform Material material;
uniform Light light;

void main()
{
    // diffuse 
    vec3 norm = normalize(normal);
    vec3 lightDir = normalize(light.position - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);
    // diffuse and ambient
    vec4 ambient;
    vec4 diffuse;
    
    if (material.useDiffuse == 1)
    {
        vec4 textureColor = vec4(vec3(texture(material.diffuse, texCoord)), 1.0);
        ambient = light.ambient * textureColor * material.ambientColor;
        diffuse = light.diffuse * diff * textureColor * material.diffuseColor;
    }
    else
    {
        ambient = light.ambient * material.ambientColor;
        diffuse = light.diffuse * diff * material.diffuseColor;
    }

    // specular
    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec4 specular;
    if (material.useSpecular == 1)
    {
        specular = light.specular * spec * vec4(vec3(texture(material.specular, texCoord)), 1.0) * material.specularColor;
    }
    else
    {
        specular = light.specular * spec * material.specularColor;
    }
    
    vec4 result = ambient + diffuse + specular;
    FragColor = result;
}
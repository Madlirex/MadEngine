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

struct DirLight {
    vec3 direction;
    
    vec4 ambient;
    vec4 diffuse;
    vec4 specular;
};

struct PointLight {
    vec3 position;
    
    float constant;
    float linear;
    float quadratic;
    
    vec4 ambient;
    vec4 diffuse;
    vec4 specular;
};

struct SpotLight {
    vec3 position;
    vec3 direction;
    float cutOff;
    float outerCutOff;

    vec4 ambient;
    vec4 diffuse;
    vec4 specular;

    float constant;
    float linear;
    float quadratic;
};

out vec4 FragColor;

in vec2 texCoord;
in vec3 Normal;
in vec3 FragPos;

uniform vec3 viewPos;

uniform Material material;

#define MAX_LIGHT_COUNT 128
uniform DirLight dirLights[MAX_LIGHT_COUNT];
uniform PointLight pointLights[MAX_LIGHT_COUNT];
uniform SpotLight spotLights[MAX_LIGHT_COUNT];

uniform int dirLightCount;
uniform int pointLightCount;
uniform int spotLightCount;

vec4 CalcDirLight(DirLight light, vec3 normal, vec3 viewDir)
{
    vec3 lightDir = normalize(-light.direction);
    float diff = max(dot(normal, lightDir), 0.0);
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
    vec3 reflectDir = reflect(-lightDir, normal);
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
    return result;
}

vec4 CalcPointLight(PointLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec3 lightDir = normalize(light.position - fragPos);
    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    // specular shading
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    // attenuation
    float distance = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));
    
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
    vec4 specular;
    if (material.useSpecular == 1)
    {
        specular = light.specular * spec * vec4(vec3(texture(material.specular, texCoord)), 1.0) * material.specularColor;
    }
    else
    {
        specular = light.specular * spec * material.specularColor;
    }
    ambient *= attenuation;
    diffuse *= attenuation;
    specular *= attenuation;
    return (ambient + diffuse + specular);
}

vec4 CalcSpotLights(SpotLight light, vec3 normal, vec3 fragPos, vec3 viewDir)
{
    vec4 ambient;
    //diffuse
    vec3 lightDir = normalize(light.position - fragPos);
    float diff = max(dot(normal, lightDir), 0.0);
    vec4 diffuse;

    //specular
    vec3 reflectDir = reflect(-lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    vec4 specular;

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
    if (material.useSpecular == 1)
    {
        specular = light.specular * spec * vec4(vec3(texture(material.specular, texCoord)), 1.0) * material.specularColor;
    }
    else
    {
        specular = light.specular * spec * material.specularColor;
    }

    //attenuation
    float distance = length(light.position - fragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));

    //spotlight intensity
    float theta = dot(lightDir, normalize(-light.direction));
    float epsilon = light.cutOff - light.outerCutOff;
    float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);

    ambient  *= attenuation * intensity;
    diffuse  *= attenuation * intensity;
    specular *= attenuation * intensity;
    
    vec4 result = ambient + diffuse + specular;
    return result;
}

void main()
{
    vec3 norm = normalize(Normal);
    vec3 viewDir = normalize(viewPos - FragPos);
    vec4 result = vec4(0.0);
    
    //Dir lights
    for(int i = 0; i < dirLightCount; i++)
    {
        result += CalcDirLight(dirLights[i], norm, viewDir);
    }
    //Point lights
    for(int i = 0; i < pointLightCount; i++)
    {
        result += CalcPointLight(pointLights[i], norm, FragPos, viewDir);
    }
    
    for(int i = 0; i < spotLightCount; i++)
    {
        result += CalcSpotLights(spotLights[i], norm, FragPos, viewDir);
    }
    
    FragColor = result;
}
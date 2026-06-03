#version 330 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec2 aTexCoord;
layout (location = 2) in vec3 aNormal;

uniform mat4 transform;
uniform mat4 view;
uniform mat4 projection;

out vec2 texCoord;
out vec3 Normal;
out vec3 FragPos;

void main()
{
    texCoord = aTexCoord;
    Normal =  aNormal * mat3(transpose(inverse(transform)));
    
    gl_Position = vec4(aPosition, 1.0) * transform * view * projection;
    FragPos = vec3(vec4(aPosition, 1.0) * transform);
}
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace MadEngine.Core;

public class Material : Asset
{
    public Shader Shader;
    public Texture? DiffuseTexture;
    public Texture? SpecularTexture;
    
    public Vector4 AmbientColor = Vector4.One * 0.8f;
    public Vector4 DiffuseColor = Vector4.One * 0.8f;
    public Vector4 SpecularColor = Vector4.One;
    public float Shininess = 32f;

    public Material(Shader shader, Texture? diffuseTexture = null, Texture? specularTexture = null, Vector4? ambientColor = null, Vector4? diffuseColor = null, Vector4? specularColor = null, float shininess = 32f)
    {
        Shader = shader;
        DiffuseTexture = diffuseTexture;
        SpecularTexture = specularTexture;
        AmbientColor = ambientColor ?? AmbientColor;
        DiffuseColor = diffuseColor ?? DiffuseColor;
        SpecularColor = specularColor ?? SpecularColor;
        Shininess = shininess;
    }

    public void Draw()
    {
        Shader.Use();
        
        if (DiffuseTexture != null)
        {
            DiffuseTexture.Bind();
            Shader.SetInt("material.diffuse", 0);
        }

        if (SpecularTexture != null)
        {
            SpecularTexture.Bind(TextureUnit.Texture1);
            Shader.SetInt("material.specular", 1);
        }
        
        Shader.SetInt("material.useDiffuse", DiffuseTexture != null ? 1 : 0);
        Shader.SetInt("material.useSpecular", SpecularTexture != null ? 1 : 0);
        
        Shader.SetVector4("material.ambientColor", AmbientColor);
        Shader.SetVector4("material.diffuseColor", DiffuseColor);
        Shader.SetVector4("material.specularColor", SpecularColor);
        Shader.SetFloat("material.shininess", Shininess);
    }
}
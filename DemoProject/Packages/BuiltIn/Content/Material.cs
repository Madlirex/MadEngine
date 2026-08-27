using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace MadEngine.Core;

public class Material : Asset
{
    public Shader Shader = Defaults.LitShader;
    
    public Dictionary<string, Texture> Textures { get; } = new();
    public Dictionary<string, Vector4> Vectors { get; } = new();
    public Dictionary<string, float> Floats { get; } = new();
    public Dictionary<string, int> Ints { get; } = new();

    public override string Name { get; set; } = "NewMaterial";
    public override string Extension => ".mat";

    public Material() { }

    public Material(Shader shader)
    {
        Shader = shader;
    }
    
    public void SetTexture(string name, Texture? texture)
    {
        if (texture == null)
        {
            Textures.Remove(name);
        }
        else
        {
            Textures[name] = texture;
        }
    }

    public void SetVector(string name, Vector4 vector) => Vectors[name] = vector;
    public void SetFloat(string name, float value) => Floats[name] = value;
    public void SetInt(string name, int value) => Ints[name] = value;
    
    public void Apply()
    {
        if (Shader == null) return;
        Shader.Use();
        
        foreach (var kvp in Vectors) Shader.SetVector4(kvp.Key, kvp.Value);
        foreach (var kvp in Floats)  Shader.SetFloat(kvp.Key, kvp.Value);
        foreach (var kvp in Ints)    Shader.SetInt(kvp.Key, kvp.Value);
        
        int textureUnitIndex = 0;
        
        foreach (var kvp in Textures)
        {
            TextureUnit unit = TextureUnit.Texture0 + textureUnitIndex;
            
            kvp.Value.Bind(unit);
            Shader.SetInt(kvp.Key, textureUnitIndex);
            Shader.SetInt(kvp.Key + "_Enabled", 1);

            textureUnitIndex++;
        }
        
        for (int i = textureUnitIndex; i < 8; i++) 
        {
            GL.ActiveTexture(TextureUnit.Texture0 + i);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }
    }
}

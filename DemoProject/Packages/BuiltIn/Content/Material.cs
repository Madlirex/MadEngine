using System.Runtime.CompilerServices;
using MadEditor;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace MadEngine.Core;

public class Material : Asset
{
    public Shader? Shader = null;

    public string Prefix = "material.";
    
    [ShowInInspector] public Dictionary<string, Texture?> Textures = new();
    [ShowInInspector] public Dictionary<string, Vector4> Vectors = new();
    [ShowInInspector] public Dictionary<string, float> Floats = new();
    [ShowInInspector] public Dictionary<string, int> Ints = new();

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
        foreach (var kvp in Vectors)
        {
            //if (kvp.Value == null) continue;
            Shader.SetVector4(Prefix + kvp.Key, kvp.Value);
        }
        foreach (var kvp in Floats)
        {
            //if (kvp.Value == null) continue;
            Shader.SetFloat(Prefix + kvp.Key, kvp.Value);
        }
        foreach (var kvp in Ints)
        {
            //if (kvp.Value == null) continue;
            Shader.SetInt(Prefix + kvp.Key, kvp.Value);
        }
        
        int textureUnitIndex = 0;
        
        foreach (var kvp in Textures)
        {
            if(kvp.Value == null) continue;
            
            TextureUnit unit = TextureUnit.Texture0 + textureUnitIndex;
            
            kvp.Value.Bind(unit);
            Shader.SetInt(Prefix + kvp.Key, textureUnitIndex);
            Shader.SetInt(Prefix + kvp.Key + "_Enabled", 1);

            textureUnitIndex++;
        }
        
        var diffuse = Textures.GetValueOrDefault("diffuse");
        
        diffuse?.Bind(TextureUnit.Texture0);
    }
}

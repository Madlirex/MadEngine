using System.Numerics;

namespace MadEngine;

public class Material
{
    public Shader Shader;
    public Vector4 Color;

    public Material(Shader shader, Vector4 color)
    {
        Shader = shader;
        Color = color;
    }

    public void Draw()
    {
        Shader.Use();
        Shader.SetVector4("uColor", Color);
    }
}
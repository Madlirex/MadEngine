using System.Numerics;
using OpenTK.Graphics.OpenGL4;

namespace MadEngine;

public class Material
{
    public Shader Shader;
    public Texture Texture;
    public Vector4 Color;

    public Material(Shader shader, Texture texture, Vector4 color)
    {
        Shader = shader;
        Texture = texture;
        Color = color;
    }

    public void Draw()
    {
        Shader.Use();
        Shader.SetVector4("uColor", Color);
    }
}
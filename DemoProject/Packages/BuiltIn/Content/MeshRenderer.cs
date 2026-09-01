using MadEngine.Core;
using OpenTK.Mathematics;

namespace MadEngine;

public class MeshRenderer : Renderer
{
    public Mesh? Mesh = Defaults.Cube;
    public Material? Material = Defaults.LitMaterial;

    public override void Draw(Matrix4 view, Matrix4 projection)
    {
        Console.WriteLine(GameObject);
        if (Mesh == null || Material?.Shader == null) return;
        if (GameObject == null) return;
        
        Material.Shader.Use();
        Material.Shader.SetMatrix4("transform", GameObject.Transform.GetWorldMatrix());
        Material.Shader.SetMatrix4("view", view);
        Material.Shader.SetMatrix4("projection", projection);

        Material.Apply();
        
        Mesh.Draw();
    }
}
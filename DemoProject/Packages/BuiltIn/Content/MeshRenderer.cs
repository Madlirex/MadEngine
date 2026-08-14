using MadEngine.Core;
using OpenTK.Mathematics;

namespace MadEngine;

public class MeshRenderer : Renderer
{
    public required Mesh Mesh;
    public required Material Material;

    public override void Draw(Matrix4 view, Matrix4 projection)
    {
        Material.Shader.Use();
        Material.Shader.SetMatrix4("transform", GameObject.Transform.GetWorldMatrix());
        Material.Shader.SetMatrix4("view", view);
        Material.Shader.SetMatrix4("projection", projection);

        Material.Draw();
        
        Mesh.Draw();
    }
}
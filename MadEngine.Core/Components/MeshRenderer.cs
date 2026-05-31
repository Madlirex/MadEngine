using OpenTK.Mathematics;

namespace MadEngine.Core;

public class MeshRenderer : Component
{
    public Mesh Mesh;
    public Material Material;

    public MeshRenderer(Mesh mesh, Material mat)
    {
        Mesh = mesh;
        Material = mat;
    }

    public void Draw(Matrix4 view, Matrix4 projection)
    {
        Material.Shader.Use();
        Material.Shader.SetMatrix4("transform", GameObject.Transform.GetWorldMatrix());
        Material.Shader.SetMatrix4("view", view);
        Material.Shader.SetMatrix4("projection", projection);

        Material.Draw();
        
        Mesh.Draw();
    }
}
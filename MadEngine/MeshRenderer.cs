namespace MadEngine;

public class MeshRenderer
{
    public GameObject? GameObject;
    public Mesh Mesh;
    public Material Material;

    public MeshRenderer(Mesh mesh, Material mat)
    {
        Mesh =  mesh;
        Material = mat;
    }

    public void Draw()
    {
        Material.Draw();
        
        Material.Shader.SetMatrix4("transform", GameObject.Transform.GetModuleMatrix());
        
        Mesh.Draw();
    }
}
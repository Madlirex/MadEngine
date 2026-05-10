namespace MadEngine;

public class MeshRenderer
{
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
        Mesh.Draw();
    }
}
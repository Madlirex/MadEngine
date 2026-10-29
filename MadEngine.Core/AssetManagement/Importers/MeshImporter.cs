namespace MadEngine.Core.AssetManagement;

public class MeshImporter : Importer<Mesh>
{
    public override string Name => "MeshImporter";
    public override string[] Extensions => [".mesh"];

    public override Mesh Import(string path)
    {
        Mesh mesh = base.Import(path);
        mesh.Initialize();
        return mesh;
    }
}
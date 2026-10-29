namespace MadEngine.Core.AssetManagement;

public class ShaderImporter : Importer<Shader>
{
    public override string Name => "ShaderImporter";
    public override string[] Extensions => [".shader"];

    public override Shader Import(string path)
    {
        Shader shader = base.Import(path);
        shader.Initialize();
        return shader;
    }
}
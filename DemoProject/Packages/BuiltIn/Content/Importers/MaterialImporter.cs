namespace MadEngine.Core.AssetManagement;

public class MaterialImporter : Importer<Material>
{
    public override string Name => "MaterialImporter";
    public override string[] Extensions => [".mat"];
}
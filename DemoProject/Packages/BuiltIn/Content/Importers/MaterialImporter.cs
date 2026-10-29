namespace MadEngine.Core.AssetManagement;

public class MaterialImporter : DefaultAssetImporter<Material>
{
    public override string Name => "MaterialImporter";
    public override string[] Extensions => [".mat"];
}
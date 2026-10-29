namespace MadEngine.Core.AssetManagement;

public class Texture2DImporter : DefaultAssetImporter<Texture2D>
{
    public override string Name => "TextureImporter";
    public override string[] Extensions => [".tex"];
}
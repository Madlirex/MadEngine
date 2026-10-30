namespace MadEngine.Core.AssetManagement;

public class Texture2DImporter : Importer<Texture2D>
{
    public override string Name => "TextureImporter";
    public override string[] Extensions => [".tex"];
}
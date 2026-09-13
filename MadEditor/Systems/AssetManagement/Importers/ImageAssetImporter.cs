using MadEngine.Core;

namespace MadEditor;

public class ImageAssetImporter : EmptyAssetImporter<ImageAsset>
{
    public override string Name => "ImageImporter";
    public override string[] Extensions => [".png", ".jpg", ".jpeg"];
}
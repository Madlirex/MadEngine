using MadEngine.Core;

namespace MadEditor;

public class ImageAssetImporter : Importer<ImageAsset>
{
    public override string Name => "ImageImporter";
    public override string[] Extensions => [".png", ".jpg", ".jpeg"];
    public override void Save(ImageAsset asset)
    {
        
    }

    public override ImageAsset Initialize(string path)
    {
        return new ImageAsset() {Name = Path.GetFileNameWithoutExtension(path)};
    }

    public override ImageAsset Initialize(AssetMeta meta)
    {
        return new ImageAsset { Guid = meta.Guid, Name = meta.Name };
    }

    public override ImageAsset Import(string path)
    {
        return (ImageAsset)AssetRegistry.GetAsset(AssetRegistry.GetGuid(path));
    }
}
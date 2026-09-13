using MadEngine;

namespace MadEditor;

public class FolderAssetImporter : EmptyAssetImporter<FolderAsset>
{
    public override string Name => "FolderImporter";
    public override string[] Extensions => [""];

    public override void Save(FolderAsset asset)
    {
        if(!Directory.Exists(asset.AbsolutePath)) Directory.CreateDirectory(asset.AbsolutePath);
    }
}
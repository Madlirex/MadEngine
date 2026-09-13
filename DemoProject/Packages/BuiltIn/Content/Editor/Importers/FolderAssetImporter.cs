using MadEngine;

namespace MadEditor;

public class FolderAssetImporter : EmptyAssetImporter<FolderAsset>
{
    public override string Name => "FolderImporter";
    public override string[] Extensions => [""];
}
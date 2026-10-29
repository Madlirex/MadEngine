using System.Text.Json.Nodes;

namespace MadEngine.Core.AssetManagement;

public class DefaultAssetImporter<T> : Importer<T> where T : Asset, new()
{
    public override string Name => "AssetImporter";
    public override string[] Extensions => [];

    public override T Import(string path)
    {
        
    }
}
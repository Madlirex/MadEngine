using MadEngine.Core;

namespace MadEditor;

public class EmptyAssetImporter<T> : Importer<T> where T : Asset, new()
{
    public override string Name => "EmptyImporter";
    public override string[] Extensions => [];
    public override void Save(T asset)
    {
        
    }

    public override T Initialize(string path)
    {
        return new T() {Name = Path.GetFileNameWithoutExtension(path)};
    }

    public override T Initialize(AssetMeta meta)
    {
        return new T { Guid = meta.Guid, Name = meta.Name };
    }

    public override T Import(string path)
    {
        return (T)AssetRegistry.GetAsset(AssetRegistry.GetGuid(path));
    }
}
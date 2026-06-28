using MadEngine.Core;

namespace MadEditor;

public interface IAssetImporter
{
    public string Name { get; }
    public IReadOnlyList<string> Extensions { get; }
    
    public Asset Load(string path);
    public Asset Load(AssetMeta meta);
    public void Save(Asset asset, string path);
}

public abstract class AssetImporter<TAsset> : IAssetImporter where TAsset : Asset
{
    public abstract string Name { get; }

    public abstract IReadOnlyList<string> Extensions { get; }

    public abstract TAsset Load(string path);

    public abstract TAsset Load(AssetMeta meta);

    public abstract void Save(TAsset asset, string path);

    Asset IAssetImporter.Load(string path) => Load(path);

    Asset IAssetImporter.Load(AssetMeta meta) => Load(meta);

    void IAssetImporter.Save(Asset asset, string path) => Save((TAsset)asset, path);
}
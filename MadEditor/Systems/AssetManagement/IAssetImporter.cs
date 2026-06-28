using MadEngine.Core;

namespace MadEditor;

public interface IAssetImporter
{
    public string Name { get; }
    public IReadOnlyList<string> Extensions { get; }

    public Asset Instantiate(string path);
    public Asset Instantiate(AssetMeta meta);
    
    public void Load(Asset asset);
    public void Save(Asset asset, string path);
}

public abstract class AssetImporter<TAsset> : IAssetImporter where TAsset : Asset
{
    public abstract string Name { get; }

    public abstract IReadOnlyList<string> Extensions { get; }

    public abstract TAsset Instantiate(string path);
    public abstract TAsset Instantiate(AssetMeta meta);
    
    public abstract void Load(TAsset asset);
    

    public abstract void Save(TAsset asset, string path);

    Asset IAssetImporter.Instantiate(string path) => Instantiate(path);
    Asset IAssetImporter.Instantiate(AssetMeta meta) => Instantiate(meta);

    void IAssetImporter.Load(Asset asset) => Load((TAsset)asset);
    

    void IAssetImporter.Save(Asset asset, string path) => Save((TAsset)asset, path);
}
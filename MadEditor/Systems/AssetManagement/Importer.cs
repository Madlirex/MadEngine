using MadEngine.Core;

namespace MadEditor;

public interface IAssetImporter
{
    public Type Type { get; }
    public string Name { get; }
    public string Extension { get; }

    public Asset Initialize(string path);
    public Asset Initialize(AssetMeta meta);
    public Asset Import(string path);
    
    public void Save(Asset asset);
}

public abstract class Importer<T> : IAssetImporter where T : Asset
{
    public Type Type => typeof(T);
    public abstract string Name { get; }
    public abstract string Extension { get; }
    public abstract T Initialize(string path);
    public abstract T Initialize(AssetMeta meta);
    public abstract T Import(string path);
    
    public abstract void Save(T asset);

    Asset IAssetImporter.Initialize(string path) => Initialize(path);
    Asset IAssetImporter.Initialize(AssetMeta meta) => Initialize(meta);
    
    Asset IAssetImporter.Import(string path) => Import(path);
    
    void IAssetImporter.Save(Asset asset) => Save((T)asset);
}
namespace MadEngine.Core.AssetManagement;

public class EmptyAssetImporter<T> : Importer<T> where T : Asset, new()
{
    public override string Name => "EmptyImporter";
    public override string[] Extensions => [];
    public override void Save(T asset)
    {
        
    }

    public override void Pack(T asset, string targetPath)
    {
        string? targetDirectory = Path.GetDirectoryName(targetPath);
        if (!string.IsNullOrEmpty(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory);
        }
        
        if (File.Exists(targetPath))
        {
            DateTime sourceTime = File.GetLastWriteTimeUtc(asset.AbsolutePath);
            DateTime targetTime = File.GetLastWriteTimeUtc(targetPath);

            if (sourceTime <= targetTime) return;
        }
        
        File.Copy(asset.AbsolutePath, targetPath, true);
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
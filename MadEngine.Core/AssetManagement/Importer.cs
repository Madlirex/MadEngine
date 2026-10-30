using System.Text.Json.Nodes;

namespace MadEngine.Core.AssetManagement;

public interface IAssetImporter
{
    public Type Type { get; }
    public string Name { get; }
    public string[] Extensions { get; }

    public Asset Initialize(string path);
    public Asset Initialize(AssetMeta meta);
    public Asset Import(string path);
    
    public void Save(Asset asset);
    public void Pack(Asset asset, string path);
    
    public string GetJsonString(Asset asset);
}

public abstract class Importer<T> : IAssetImporter where T : Asset, new()
{
    public Type Type => typeof(T);
    public abstract string Name { get; }
    public abstract string[] Extensions { get; }
    public virtual T Initialize(string path) => SerializerRegistry.GetSerializer(typeof(T))!.Deserialize(Unpack(path)) as T ?? new T();
    public virtual T Initialize(AssetMeta meta) => new() { Guid = meta.Guid, Name = meta.Name };
    public virtual T Import(string path)
    {
        JsonNode json = Unpack(path);
        
        Guid guid = json["$guid"]!.GetValue<Guid>();
        T asset = (T)AssetRegistry.GetAsset(guid)!;

        SerializerRegistry.GetClassSerializer(typeof(T))!.DeserializeInto(asset, json["$data"]!);
        return asset;
    }

    public virtual void Save(T asset)
    {
        File.WriteAllText(asset.AbsolutePath, GetJsonString(asset));
    }

    public virtual void Pack(T asset, string targetPath)
    {
        AssetPacker.PackAsset(GetJsonString(asset), targetPath);
    }

    public virtual string GetJsonString(T asset)
    {
        JsonObject jsonObject = (JsonObject)SerializerRegistry.GetSerializer(typeof(T))!.Serialize(asset);
        return jsonObject.ToJsonString(SerializerSettings.SerializerOptions);
    }

    Asset IAssetImporter.Initialize(string path) => Initialize(path);
    Asset IAssetImporter.Initialize(AssetMeta meta) => Initialize(meta);
    
    Asset IAssetImporter.Import(string path) => Import(path);
    
    void IAssetImporter.Save(Asset asset) => Save((T)asset);
    string IAssetImporter.GetJsonString(Asset asset) => GetJsonString((T)asset);

    protected JsonNode Unpack(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Asset target missing: {path}");
        
        string data = Path.GetExtension(path) == SerializerSettings.PackedExtension ? AssetPacker.UnpackAsset(path) : File.ReadAllText(path);
        return JsonNode.Parse(data)!;
    }

    void IAssetImporter.Pack(Asset asset, string path) => Pack((T)asset, path);
}
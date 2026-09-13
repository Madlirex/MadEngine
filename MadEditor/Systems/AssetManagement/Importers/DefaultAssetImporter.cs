using System.Text.Json.Nodes;
using MadEngine.Core;

namespace MadEditor;

public class DefaultAssetImporter<T> : Importer<T> where T : Asset, new()
{
    public override string Name => "AssetImporter";
    public override string[] Extensions => [];
    public override void Save(T asset)
    {
        JsonObject jsonObject = (JsonObject)SerializerRegistry.GetSerializer(typeof(T))!.Serialize(asset);
        File.WriteAllText(asset.AbsolutePath, jsonObject.ToJsonString(SerializerSettings.SerializerOptions));
    }

    public override T Initialize(string path)
    {
        string data = File.ReadAllText(path);
        JsonNode json = JsonNode.Parse(data)!;
        
        return SerializerRegistry.GetSerializer(typeof(T))!.Deserialize(json) as T ?? new T();
    }

    public override T Initialize(AssetMeta meta)
    {
        return new T { Guid = meta.Guid, Name = meta.Name };
    }

    public override T Import(string path)
    {
        string data = File.ReadAllText(path);
        JsonNode json = JsonNode.Parse(data)!;
        
        Guid guid = json["$guid"]!.GetValue<Guid>();
        T material = (T)AssetRegistry.GetAsset(guid);

        SerializerRegistry.GetClassSerializer(typeof(T))!.DeserializeInto(material, json["$data"]!);
        return material;
    }
}
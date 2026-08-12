using System.Text.Json.Nodes;
using MadEngine.Core;

namespace MadEditor;

public class MaterialImporter : Importer<Material>
{
    public override string Name => "MaterialImporter";
    public override string Extension => ".mat";
    public override void Save(Material asset)
    {
        JsonObject jsonObject = (JsonObject)SerializerRegistry.GetSerializer(typeof(Material))!.Serialize(asset);
        File.WriteAllText(asset.AbsolutePath, jsonObject.ToJsonString(SerializerSettings.SerializerOptions));
    }

    public override Material Initialize(string path)
    {
        string data = File.ReadAllText(path);
        JsonNode json = JsonNode.Parse(data)!;
        
        return SerializerRegistry.GetSerializer(typeof(Material))!.Deserialize(json) as Material ?? new Material();
    }

    public override Material Initialize(AssetMeta meta)
    {
        return new Material { Guid = meta.Guid, Name = meta.Name };
    }

    public override Material Import(string path)
    {
        string data = File.ReadAllText(path);
        JsonNode json = JsonNode.Parse(data)!;
        
        Guid guid = json["$guid"]!.GetValue<Guid>();
        Material material = (Material)AssetRegistry.GetAsset(guid);

        SerializerRegistry.GetClassSerializer(typeof(Material))!.DeserializeInto(material, json["$data"]!);
        return material;
    }
}
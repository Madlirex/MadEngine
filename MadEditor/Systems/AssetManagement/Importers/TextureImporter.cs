using System.Text.Json.Nodes;
using MadEngine.Core;

namespace MadEditor;

public class TextureImporter : Importer<Texture>
{
    public override string Name => "TextureImporter";
    public override string Extension => ".tex";
    public override void Save(Texture asset)
    {
        JsonObject jsonObject = (JsonObject)SerializerRegistry.GetSerializer(typeof(Texture))!.Serialize(asset);
        File.WriteAllText(asset.AbsolutePath, jsonObject.ToJsonString(SerializerSettings.SerializerOptions));
    }

    public override Texture Initialize(string path)
    {
        string data = File.ReadAllText(path);
        JsonNode json = JsonNode.Parse(data)!;
        
        return SerializerRegistry.GetSerializer(typeof(Texture))!.Deserialize(json) as Texture ?? new Texture();
    }

    public override Texture Initialize(AssetMeta meta)
    {
        return new Texture { Guid = meta.Guid, Name = meta.Name };
    }

    public override Texture Import(string path)
    {
        string data = File.ReadAllText(path);
        JsonNode json = JsonNode.Parse(data)!;
        
        Guid guid = json["$guid"]!.GetValue<Guid>();
        Texture texture = (Texture)AssetRegistry.GetAsset(guid);

        SerializerRegistry.GetClassSerializer(typeof(Texture))!.DeserializeInto(texture, json["$data"]!);
        return texture;
    }
}
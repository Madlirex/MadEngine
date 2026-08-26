using System;
using System.Text.Json.Nodes;
using MadEngine;
using MadEngine.Core;

namespace MadEditor;

public class Texture2DImporter : Importer<Texture2D>
{
    public override string Name => "TextureImporter";
    public override string Extension => ".tex";
    public override void Save(Texture2D asset)
    {
        JsonObject jsonObject = (JsonObject)SerializerRegistry.GetSerializer(typeof(Texture2D))!.Serialize(asset);
        File.WriteAllText(asset.AbsolutePath, jsonObject.ToJsonString(SerializerSettings.SerializerOptions));
    }

    public override Texture2D Initialize(string path)
    {
        string data = File.ReadAllText(path);
        JsonNode json = JsonNode.Parse(data)!;
        
        return SerializerRegistry.GetSerializer(typeof(Texture2D))!.Deserialize(json) as Texture2D ?? new Texture2D();
    }

    public override Texture2D Initialize(AssetMeta meta)
    {
        return new Texture2D { Guid = meta.Guid, Name = meta.Name };
    }

    public override Texture2D Import(string path)
    {
        string data = File.ReadAllText(path);
        JsonNode json = JsonNode.Parse(data)!;
        
        Guid guid = json["$guid"]!.GetValue<Guid>();
        Texture2D texture = (Texture2D)AssetRegistry.GetAsset(guid);

        SerializerRegistry.GetClassSerializer(typeof(Texture2D))!.DeserializeInto(texture, json["$data"]!);
        return texture;
    }
}
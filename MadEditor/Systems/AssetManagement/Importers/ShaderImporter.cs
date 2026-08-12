using System.Text.Json.Nodes;
using MadEngine.Core;

namespace MadEditor;

public class ShaderImporter : Importer<Shader>
{
    public override string Name => "ShaderImporter";
    public override string Extension => ".shader";
    public override void Save(Shader asset)
    {
        JsonObject jsonObject = (JsonObject)SerializerRegistry.GetSerializer(typeof(Shader))!.Serialize(asset);
        File.WriteAllText(asset.AbsolutePath, jsonObject.ToJsonString(SerializerSettings.SerializerOptions));
    }

    public override Shader Initialize(string path)
    {
        string data = File.ReadAllText(path);
        JsonNode json = JsonNode.Parse(data)!;
        
        return SerializerRegistry.GetSerializer(typeof(Shader))!.Deserialize(json) as Shader ?? new Shader();
    }

    public override Shader Initialize(AssetMeta meta)
    {
        return new Shader { Guid = meta.Guid, Name = meta.Name };
    }

    public override Shader Import(string path)
    {
        string data = File.ReadAllText(path);
        JsonNode json = JsonNode.Parse(data)!;
        
        Guid guid = json["$guid"]!.GetValue<Guid>();
        Shader shader = (Shader)AssetRegistry.GetAsset(guid);

        SerializerRegistry.GetClassSerializer(typeof(Shader))!.DeserializeInto(shader, json["$data"]!);
        return shader;
    }
}
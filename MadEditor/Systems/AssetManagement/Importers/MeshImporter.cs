using System.Text.Json.Nodes;
using MadEngine.Core;

namespace MadEditor;

public class MeshImporter : Importer<Mesh>
{
    public override string Name => "MeshImporter";
    public override string Extension => ".mesh";
    public override void Save(Mesh asset)
    {
        JsonObject jsonObject = (JsonObject)SerializerRegistry.GetSerializer(typeof(Mesh))!.Serialize(asset);
        File.WriteAllText(asset.AbsolutePath, jsonObject.ToJsonString(SerializerSettings.SerializerOptions));
    }

    public override Mesh Initialize(string path)
    {
        string data = File.ReadAllText(path);
        JsonNode json = JsonNode.Parse(data)!;
        
        return SerializerRegistry.GetSerializer(typeof(Mesh))!.Deserialize(json) as Mesh ?? new Mesh();
    }

    public override Mesh Initialize(AssetMeta meta)
    {
        return new Mesh { Guid = meta.Guid, Name = meta.Name };
    }

    public override Mesh Import(string path)
    {
        string data = File.ReadAllText(path);
        JsonNode json = JsonNode.Parse(data)!;
        
        Guid guid = json["$guid"]!.GetValue<Guid>();
        Mesh mesh = (Mesh)AssetRegistry.GetAsset(guid);

        SerializerRegistry.GetClassSerializer(typeof(Mesh))!.DeserializeInto(mesh, json);
        return mesh;
    }
}
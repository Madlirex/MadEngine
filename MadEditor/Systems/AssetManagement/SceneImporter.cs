using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;

namespace MadEditor;

public class SceneImporter : AssetImporter<Scene>
{
    public override string Name => "SceneImporter";
    public override IReadOnlyList<string> Extensions => [".madscene"];
    public override Scene Load(string path)
    {
        return Deserialize(path);
    }

    public override Scene Load(AssetMeta meta)
    {
        return Deserialize(meta.Path);
    }

    private Scene Deserialize(string path)
    {
        string json = File.ReadAllText(path);
        JsonObject obj = JsonSerializer.Deserialize<JsonObject>(json)!;
        
        return (Scene)SerializerRegistry.GetSerializer<Scene>().DeserializeObject(obj);
    }

    public override void Save(Scene asset, string path)
    {
        Console.WriteLine($"Saving {path}");
        var serializer = SerializerRegistry.GetSerializer<Scene>();
        
        JsonObject json = SerializerRegistry.GetSerializer<Scene>().SerializeObject(asset);
        File.WriteAllText(path, json.ToString());
    }
}
using System.Text.Json.Nodes;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;

namespace MadEditor;

public class SceneImporter : Importer<Scene>
{
    public override string Name => "SceneImporter";
    public override string Extension => ".madscene";
    
    public override void Save(Scene asset)
    {
        JsonObject jsonObject = (JsonObject)SerializerRegistry.GetSerializer(typeof(Scene))!.Serialize(asset);

        JsonArray objArray = new JsonArray();
        JsonArray compArray = new JsonArray();

        foreach (GameObject gameObject in asset.GameObjects)
        {
            objArray.Add(SerializerRegistry.GetSerializer(typeof(GameObject))!.Serialize(gameObject));
            
            foreach (Component component in gameObject.Components)
            {
                compArray.Add(SerializerRegistry.GetSerializer(component.GetType())!.Serialize(component));
            }
        }
        jsonObject.Add("GameObjects", objArray);
        jsonObject.Add("Components", compArray);

        File.WriteAllText(asset.AbsolutePath, jsonObject.ToJsonString(SerializerSettings.SerializerOptions));
    }

    public override Scene Initialize(string path)
    {
        string data = File.ReadAllText(path);
        JsonNode json = JsonNode.Parse(data)!;

        return SerializerRegistry.GetSerializer(typeof(Scene))!.Deserialize(json) as Scene ?? new Scene();
    }

    public override Scene Initialize(AssetMeta meta)
    {
        return new Scene {Guid = meta.Guid, Name = meta.Name};
    }

    public override Scene Import(string path)
    {
        string data = File.ReadAllText(path);
        JsonNode json = JsonNode.Parse(data)!;
        
        Guid guid = json["$guid"]!.GetValue<Guid>();

        Scene obj = (Scene)AssetRegistry.GetObject(guid)!;
        SerializerRegistry.GetClassSerializer(typeof(Scene))!.DeserializeInto(obj, json["$data"]!);
        
        return obj;
    }
}
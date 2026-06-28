using System.Text.Json;
using System.Text.Json.Nodes;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;

namespace MadEditor;

public class SceneImporter : AssetImporter<Scene>
{
    public override string Name => "SceneImporter";
    public override IReadOnlyList<string> Extensions => [".madscene"];
    public override Scene Instantiate(string path)
    {
        string text = File.ReadAllText(path);
        JsonObject obj = JsonSerializer.Deserialize<JsonObject>(text)!;
        
        JsonObject sceneJson = obj["Scene"]!.AsObject();

        Guid sceneGuid = Guid.Parse(sceneJson["Guid"]!.GetValue<string>());

        Scene scene = new Scene
        {
            Guid = sceneGuid
        };
        
        JsonObject gameObjectsJson = obj["GameObjects"]!.AsObject();

        foreach (var pair in gameObjectsJson)
        {
            Guid guid = Guid.Parse(pair.Key);

            GameObject go = new GameObject
            {
                Guid = guid
            };

            AssetRegistry.RegisterObject(go);
        }
        
        JsonObject componentsJson = obj["Components"]!.AsObject();

        foreach (var pair in componentsJson)
        {
            Guid guid = Guid.Parse(pair.Key);
            
            Console.WriteLine(pair.Value!["ComponentType"]!.GetValue<string>());
            Console.WriteLine(Type.GetType(pair.Value!["ComponentType"]!.GetValue<string>())!);
            Type type = Type.GetType(pair.Value!["ComponentType"]!.GetValue<string>())!;

            Component component = (Component)Activator.CreateInstance(type)!;

            component.Guid = guid;
            
            AssetRegistry.RegisterObject(component);
        }

        return scene;
    }

    public override Scene Instantiate(AssetMeta meta)
    {
        return Instantiate(meta.Path);
    }

    public override void Load(Scene asset)
    {
        Console.WriteLine("holaa");
        string json = File.ReadAllText(asset.Path);
        JsonObject obj = JsonSerializer.Deserialize<JsonObject>(json)!;
        
        SerializerRegistry.GetSerializer<Scene>().DeserializeIntoObject(obj, asset);
        SceneManager.Scenes.Add(asset);
        SceneManager.LoadScene(asset);
    }

    public override void Save(Scene asset, string path)
    {
        JsonObject json = SerializerRegistry.GetSerializer<Scene>().SerializeObject(asset);
        File.WriteAllText(path, json.ToString());
    }
}
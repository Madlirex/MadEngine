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
        JsonNode jsonObject = SaveToJson(asset);

        File.WriteAllText(asset.AbsolutePath, jsonObject.ToJsonString(SerializerSettings.SerializerOptions));
    }

    public JsonNode SaveToJson(Scene asset)
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
        return jsonObject;
    }

    public override Scene Initialize(string path)
    {
        string data = File.ReadAllText(path);
        JsonNode json = JsonNode.Parse(data)!;

        return InstantiateFromJson(json);
    }

    public override Scene Initialize(AssetMeta meta)
    {
        return new Scene {Guid = meta.Guid, Name = meta.Name};
    }
    
    public static void InstantiateObjects(JsonNode json)
    {
        if (json["GameObjects"] is JsonArray gameObjectsArray)
        {
            foreach (JsonNode? node in gameObjectsArray)
            {
                if (node is not JsonObject goJson) continue;

                Guid guid = goJson["$guid"]!.GetGuid();
                GameObject obj = new GameObject() {Guid = guid};
            }
        }
        if (json["Components"] is not JsonArray componentsArray)
            return;
    
        foreach (JsonNode? node in componentsArray)
        {
            if (node is not JsonObject compJson) continue;

            string typeStr = compJson["$type"]!.GetValue<string>();
            Guid guid = compJson["$guid"]!.GetGuid();

            if (string.IsNullOrEmpty(typeStr)) continue;
            Type compType = ScriptDomain.GetType(typeStr)!;
            
            if (compType is { IsAbstract: false })
            {
                Component comp = (Component)Activator.CreateInstance(compType)!;
                comp.Guid = guid;
            }
        }
    }

    public static void ImportObjects(JsonNode json, Scene scene)
    {
        if (json["GameObjects"] is JsonArray gameObjectsArray)
        {
            foreach (JsonNode? node in gameObjectsArray)
            {
                if (node is not JsonObject goJson) continue;

                Guid guid = goJson["$guid"]!.GetGuid();
                GameObject obj = (GameObject)AssetRegistry.GetObject(guid)!;
                SerializerRegistry.GetClassSerializer(typeof(GameObject))!.DeserializeInto(obj, goJson["$data"]!);
                scene.Register(obj);
            }
        }
        if (json["Components"] is not JsonArray componentsArray)
            return;
    
        foreach (JsonNode? node in componentsArray)
        {
            if (node is not JsonObject compJson) continue;
            
            Guid guid = compJson["$guid"]!.GetGuid();
            string typeStr = compJson["$type"]!.GetValue<string>();
            
            if (string.IsNullOrEmpty(typeStr)) continue;
            
            Type compType = ScriptDomain.GetType(typeStr)!;
            
            if (compType is { IsAbstract: false })
            {
                Component obj = (Component)AssetRegistry.GetObject(guid)!;
                SerializerRegistry.GetClassSerializer(compType)!.DeserializeInto(obj, compJson["$data"]!);
            }
        }
    }

    public override Scene Import(string path)
    {
        string data = File.ReadAllText(path);
        JsonNode json = JsonNode.Parse(data)!;

        Scene obj = ImportFromJson(json);
        
        return obj;
    }
    
    public Scene InstantiateFromJson(JsonNode json)
    {
        return SerializerRegistry.GetSerializer(typeof(Scene))!.Deserialize(json) as Scene ?? new Scene();
    }

    public Scene ImportFromJson(JsonNode json)
    {
        InstantiateObjects(json);
        
        Guid guid = json["$guid"]!.GetGuid();

        Scene obj = (Scene)AssetRegistry.GetObject(guid)!;
        SerializerRegistry.GetClassSerializer(typeof(Scene))!.DeserializeInto(obj, json["$data"]!);
        ImportObjects(json, obj);
        return obj;
    }
}
using System.Text.Json.Nodes;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;
using OpenTK.Mathematics;

namespace MadEditor;

public class SceneSerializer : Serializer<Scene>
{
    public override JsonObject Serialize(Scene obj)
    {
        Component[] components = obj.GameObjects.SelectMany(go => go.GetComponents<Component>()).ToArray();
        
        JsonObject objJson = new JsonObject();
        JsonObject compJson = new JsonObject();
        
        foreach (GameObject go in obj.GameObjects)
        {
            objJson[go.Guid.ToString()] = ReflectionSerializer.Serialize(go);
            foreach (Component comp in go.Components)
            {
                compJson[comp.Guid.ToString()] = ReflectionSerializer.Serialize(comp);
            }
        }

        JsonObject json = new JsonObject()
        {
            ["Scene"] = ReflectionSerializer.Serialize(obj),
            ["GameObjects"] = objJson,
            ["Components"] = compJson
        };
        
        return json;
    }

    public override Scene Deserialize(JsonObject obj)
    {
        Scene scene = new Scene();
        ReflectionSerializer.DeserializeInto(scene, obj, AssetRegistry.ObjectMap);
        return scene;
    }

    public override void DeserializeInto(JsonObject obj, Scene scene)
    {
        ReflectionSerializer.DeserializeInto(scene, obj, AssetRegistry.ObjectMap);
    }
}

public class Vector3Serializer : ValueSerializer<Vector3>
{
    public override JsonObject Serialize(Vector3 obj)
    {
        return new JsonObject()
        {
            ["X"] = obj.X,
            ["Y"] = obj.Y,
            ["Z"] = obj.Z
        };
    }

    public override Vector3 Deserialize(JsonObject obj)
    {
        return new Vector3(
            obj["X"]?.GetValue<float>() ?? 0,
            obj["Y"]?.GetValue<float>() ?? 0,
            obj["Z"]?.GetValue<float>() ?? 0
        );
    }
}

public class Vector2Serializer : ValueSerializer<Vector2>
{
    public override JsonObject Serialize(Vector2 obj)
    {
        return new JsonObject()
        {
            ["X"] = obj.X,
            ["Y"] = obj.Y,
        };
    }

    public override Vector2 Deserialize(JsonObject obj)
    {
        return new Vector2(
            obj["X"]?.GetValue<float>() ?? 0,
            obj["Y"]?.GetValue<float>() ?? 0
        );
    }
}

public class Vector4Serializer : ValueSerializer<Vector4>
{
    public override JsonObject Serialize(Vector4 obj)
    {
        return new JsonObject()
        {
            ["X"] = obj.X,
            ["Y"] = obj.Y,
            ["Z"] = obj.Z,
            ["W"] = obj.W
        };
    }

    public override Vector4 Deserialize(JsonObject obj)
    {
        return new Vector4(
            obj["X"]?.GetValue<float>() ?? 0,
            obj["Y"]?.GetValue<float>() ?? 0,
            obj["Z"]?.GetValue<float>() ?? 0,
            obj["W"]?.GetValue<float>() ?? 0
        );
    }
}

public class QuaternionSerializer : ValueSerializer<Quaternion>
{
    public override JsonObject Serialize(Quaternion obj)
    {
        return new JsonObject()
        {
            ["X"] = obj.X,
            ["Y"] = obj.Y,
            ["Z"] = obj.Z,
            ["W"] = obj.W
        };
    }

    public override Quaternion Deserialize(JsonObject obj)
    {
        return new Quaternion(
            obj["X"]?.GetValue<float>() ?? 0,
            obj["Y"]?.GetValue<float>() ?? 0,
            obj["Z"]?.GetValue<float>() ?? 0,
            obj["W"]?.GetValue<float>() ?? 0
        );
    }
}
using System.Text.Json.Nodes;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;
using OpenTK.Mathematics;

namespace MadEditor;

public class SceneSerializer : Serializer<Scene>
{
    public override JsonObject Serialize(Scene obj)
    {
        return ReflectionSerializer.Serialize(obj);
    }

    public override Scene Deserialize(JsonObject obj)
    {
        Scene scene = new();

        ReflectionSerializer.DeserializeInto(scene, obj, AssetRegistry.ObjectMap);
        return scene;
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
using System.Text.Json.Nodes;
using OpenTK.Mathematics;

namespace MadEditor;

public class BoolSerializer : Serializer<bool>
{
    public override JsonNode Serialize(bool obj) => JsonValue.Create(obj);
    public override bool Deserialize(JsonNode obj) => obj.GetValue<bool>();
}

public class IntSerializer : Serializer<int>
{
    public override JsonNode Serialize(int obj) => JsonValue.Create(obj);
    public override int Deserialize(JsonNode obj) => obj.GetValue<int>();
}

public class FloatSerializer : Serializer<float>
{
    public override JsonNode Serialize(float obj) => JsonValue.Create(obj);
    public override float Deserialize(JsonNode obj) => obj.GetValue<float>();
}

public class DoubleSerializer : Serializer<double>
{
    public override JsonNode Serialize(double obj) => JsonValue.Create(obj);
    public override double Deserialize(JsonNode obj) => obj.GetValue<double>();
}

public class LongSerializer : Serializer<long>
{
    public override JsonNode Serialize(long obj) => JsonValue.Create(obj);
    public override long Deserialize(JsonNode obj) => obj.GetValue<long>();
}

public class StringSerializer : Serializer<string>
{
    public override JsonNode Serialize(string? obj) => JsonValue.Create(obj ?? string.Empty);
    public override string Deserialize(JsonNode obj) => obj.GetValue<string>();
}

public class Vector2Serializer : Serializer<Vector2>
{
    public override JsonNode Serialize(Vector2 obj)
    {
        return new JsonArray { JsonValue.Create(obj.X), JsonValue.Create(obj.Y) };
    }

    public override Vector2 Deserialize(JsonNode obj)
    {
        if (obj is not JsonArray array || array.Count < 2) return Vector2.Zero;
        
        float x = array[0]?.GetValue<float>() ?? 0f;
        float y = array[1]?.GetValue<float>() ?? 0f;
        return new Vector2(x, y);
    }
}

public class Vector3Serializer : Serializer<Vector3>
{
    public override JsonNode Serialize(Vector3 obj)
    {
        return new JsonArray { JsonValue.Create(obj.X), JsonValue.Create(obj.Y), JsonValue.Create(obj.Z) };
    }

    public override Vector3 Deserialize(JsonNode obj)
    {
        if (obj is not JsonArray array || array.Count < 3) return Vector3.Zero;
        
        float x = array[0]?.GetValue<float>() ?? 0f;
        float y = array[1]?.GetValue<float>() ?? 0f;
        float z = array[2]?.GetValue<float>() ?? 0f;
        return new Vector3(x, y, z);
    }
}

public class Vector4Serializer : Serializer<Vector4>
{
    public override JsonNode Serialize(Vector4 obj)
    {
        return new JsonArray { JsonValue.Create(obj.X), JsonValue.Create(obj.Y), JsonValue.Create(obj.Z), JsonValue.Create(obj.W) };
    }

    public override Vector4 Deserialize(JsonNode obj)
    {
        if (obj is not JsonArray array || array.Count < 4) return Vector4.Zero;
        
        float x = array[0]?.GetValue<float>() ?? 0f;
        float y = array[1]?.GetValue<float>() ?? 0f;
        float z = array[2]?.GetValue<float>() ?? 0f;
        float w = array[3]?.GetValue<float>() ?? 0f;
        return new Vector4(x, y, z, w);
    }
}

public class QuaternionSerializer : Serializer<Quaternion>
{
    public override JsonNode Serialize(Quaternion obj)
    {
        return new JsonArray
            { JsonValue.Create(obj.X), JsonValue.Create(obj.Y), JsonValue.Create(obj.Z), JsonValue.Create(obj.W) };
    }

    public override Quaternion Deserialize(JsonNode obj)
    {
        if (obj is not JsonArray array || array.Count < 4) return Quaternion.Identity;
        
        float x = array[0]?.GetValue<float>() ?? 0f;
        float y = array[1]?.GetValue<float>() ?? 0f;
        float z = array[2]?.GetValue<float>() ?? 0f;
        float w = array[3]?.GetValue<float>() ?? 0f;
        return new Quaternion(x, y, z, w);
    }
}

public class GuidSerializer : Serializer<Guid>
{
    public override JsonNode Serialize(Guid obj) => JsonValue.Create(obj);

    public override Guid Deserialize(JsonNode obj) => obj.GetValue<Guid>();
}
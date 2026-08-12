using System.Numerics;
using System.Text.Json.Nodes;
using MadEngine.Core;
using Quaternion = OpenTK.Mathematics.Quaternion;
using Vector2 = OpenTK.Mathematics.Vector2;
using Vector3 = OpenTK.Mathematics.Vector3;
using Vector4 = OpenTK.Mathematics.Vector4;

namespace MadEditor;

public class BoolSerializer : Serializer<bool>
{
    public override JsonNode Serialize(bool obj) => JsonValue.Create(obj);
    public override bool Deserialize(JsonNode obj) => obj.GetValue<bool>();
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

public class VertexSerializer : Serializer<Vertex>
{
    public override JsonNode Serialize(Vertex obj)
    {
        JsonObject objJson = new JsonObject
        {
            ["Normal"] = SerializerRegistry.Serialize(obj.Normal),
            ["Position"] = SerializerRegistry.Serialize(obj.Position),
            ["TexCoord"] = SerializerRegistry.Serialize(obj.TexCoord)
        };
        return objJson;
    }

    public override Vertex Deserialize(JsonNode obj)
    {
        if(obj is not JsonObject objJson) return default;
        
        Vertex vertex = new Vertex
        {
            Normal = (Vector3)SerializerRegistry.Deserialize(typeof(Vector3), objJson["Normal"])!,
            Position = (Vector3)SerializerRegistry.Deserialize(typeof(Vector3), objJson["Position"])!,
            TexCoord = (Vector2)SerializerRegistry.Deserialize(typeof(Vector2), objJson["TexCoord"])!
        };
        return vertex;
    }
}

public class NumberSerializer<T> : Serializer<T> where T : INumber<T>
{
    public override JsonNode Serialize(T obj) => JsonValue.Create(obj)!;

    public override T Deserialize(JsonNode obj) => obj.GetValue<T>();
}

public class IntSerializer : NumberSerializer<int>;
public class UIntSerializer : NumberSerializer<uint>;
public class LongSerializer : NumberSerializer<long>;
public class ULongSerializer : NumberSerializer<ulong>;
public class ShortSerializer : NumberSerializer<short>;
public class UShortSerializer : NumberSerializer<ushort>;
public class ByteSerializer : NumberSerializer<byte>;
public class SByteSerializer : NumberSerializer<sbyte>;
public class FloatSerializer : NumberSerializer<float>;
public class DoubleSerializer : NumberSerializer<double>;
public class DecimalSerializer : NumberSerializer<decimal>;
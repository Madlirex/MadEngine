using System.Text.Json.Nodes;

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
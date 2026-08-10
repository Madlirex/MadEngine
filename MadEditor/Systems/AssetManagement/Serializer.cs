using System.Text.Json.Nodes;

namespace MadEditor;

public interface ISerializer
{
    public Type Type { get; }
    public JsonNode Serialize(object obj);
    public object? Deserialize(JsonNode obj);
    public void DeserializeInto(object target, JsonNode source);
}

public abstract class Serializer<T> : ISerializer
{
    public Type Type => typeof(T);
    public abstract JsonNode Serialize(T obj);
    public abstract T Deserialize(JsonNode obj);
    public abstract void DeserializeInto(T target, JsonNode source);
    
    JsonNode ISerializer.Serialize(object obj) => Serialize((T)obj);

    object? ISerializer.Deserialize(JsonNode obj) => Deserialize(obj);
    void ISerializer.DeserializeInto(object target, JsonNode source) => DeserializeInto((T)target, source);
}
using System.Text.Json.Nodes;

namespace MadEditor;

public interface ISerializer
{
    public Type Type { get; }
    public JsonNode Serialize(object obj);
    public object? Deserialize(JsonNode obj);
}

public interface IDeserializableInto
{
    public void DeserializeInto(object target, JsonNode source);
}

public abstract class Serializer<T> : ISerializer
{
    public Type Type => typeof(T);
    public abstract JsonNode Serialize(T obj);
    public abstract T Deserialize(JsonNode obj);
    
    JsonNode ISerializer.Serialize(object obj) => Serialize((T)obj);

    object? ISerializer.Deserialize(JsonNode obj) => Deserialize(obj);
}

public interface IClassSerializer
{
    public JsonNode SerializeReference(object obj);
    public object? DeserializeReference(JsonNode obj);
}

public abstract class ClassSerializer<T> : Serializer<T>, IDeserializableInto, IClassSerializer where T : class
{
    public abstract JsonNode SerializeReference(T obj);
    public abstract void DeserializeInto(T target, JsonNode source);
    public abstract T DeserializeReference(JsonNode obj);
    JsonNode IClassSerializer.SerializeReference(object obj) => SerializeReference((T)obj);
    void IDeserializableInto.DeserializeInto(object target, JsonNode source) => DeserializeInto((T)target, source);
    object? IClassSerializer.DeserializeReference(JsonNode obj) => DeserializeReference(obj);
}
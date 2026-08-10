using System.Text.Json.Nodes;

namespace MadEditor;

public interface ISerializer
{
    public Type Type { get; }
    public JsonObject Serialize(object obj);
    public object? Deserialize(JsonObject obj);
    public void DeserializeInto(object target, JsonObject souce);
}

public abstract class Serializer<T> : ISerializer
{
    public Type Type => typeof(T);
    public abstract JsonObject Serialize(T obj);
    public abstract T Deserialize(JsonObject obj);
    public abstract void DeserializeInto(T target, JsonObject souce);
    
    JsonObject ISerializer.Serialize(object obj) => Serialize((T)obj);

    object? ISerializer.Deserialize(JsonObject obj) => Deserialize(obj);
    void ISerializer.DeserializeInto(object target, JsonObject souce) => DeserializeInto((T)target, souce);
}
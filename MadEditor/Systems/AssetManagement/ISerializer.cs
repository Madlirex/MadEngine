using System.Text.Json;
using System.Text.Json.Nodes;
using MadEngine.Core;

namespace MadEditor;

public interface ISerializer
{
    public Type ObjectType { get; }
    
    JsonObject SerializeObject(object obj);
    object DeserializeObject(JsonObject obj);
    void DeserializeIntoObject(JsonObject obj, object target);
    
    public static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };
}

public abstract class Serializer<TObject> : ISerializer where TObject : MadObject
{
    public Type ObjectType => typeof(TObject);

    public abstract JsonObject Serialize(TObject obj);

    public abstract TObject Deserialize(JsonObject obj);
    public virtual void DeserializeInto(JsonObject obj, TObject target)
    {
        throw new NotSupportedException($"{GetType().Name} doesn't support DeserializeInto.");
    }

    JsonObject ISerializer.SerializeObject(object obj) => Serialize((TObject)obj);
    object ISerializer.DeserializeObject(JsonObject obj) => Deserialize(obj);
    void ISerializer.DeserializeIntoObject(JsonObject obj, object target) => DeserializeInto(obj, (TObject)target);
}

public abstract class ValueSerializer<T> : ISerializer
{
    public Type ObjectType => typeof(T);
    public abstract JsonObject Serialize(T obj);
    public abstract T Deserialize(JsonObject obj);

    public virtual void DeserializeInto(JsonObject obj, T target)
    {
        throw new NotSupportedException($"{GetType().Name} doesn't support DeserializeInto.");
    }
    
    JsonObject ISerializer.SerializeObject(object obj) => Serialize((T)obj);
    object ISerializer.DeserializeObject(JsonObject obj) => Deserialize(obj)!;
    void ISerializer.DeserializeIntoObject(JsonObject obj, object target) => DeserializeInto(obj, (T)target);
}
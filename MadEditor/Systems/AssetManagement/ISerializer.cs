using System.Text.Json.Nodes;
using MadEngine.Core;

namespace MadEditor;

public interface ISerializer
{
    public Type ObjectType { get; }
    
    JsonObject Serialize(object obj);
    object Deserialize(JsonObject obj);
}

public interface IObjectSerializer : ISerializer
{
    JsonObject Serialize(MadObject obj);
    new MadObject Deserialize(JsonObject obj);
    
    JsonObject ISerializer.Serialize(object obj) => Serialize(obj);
    object ISerializer.Deserialize(JsonObject obj) => Deserialize(obj);
}

public abstract class Serializer<TObject> : IObjectSerializer where TObject : MadObject
{
    public Type ObjectType => typeof(TObject);

    public abstract JsonObject Serialize(TObject obj);

    public abstract TObject Deserialize(JsonObject obj);

    JsonObject IObjectSerializer.Serialize(MadObject obj) => Serialize((TObject)obj);
    MadObject IObjectSerializer.Deserialize(JsonObject obj) => Deserialize(obj);
}

public abstract class ValueSerializer<T> : ISerializer
{
    public Type ObjectType => typeof(T);
    public abstract JsonObject Serialize(T obj);
    public abstract T Deserialize(JsonObject obj);
    
    JsonObject ISerializer.Serialize(object obj) => Serialize((T)obj);
    object ISerializer.Deserialize(JsonObject obj) => Deserialize(obj)!;
}
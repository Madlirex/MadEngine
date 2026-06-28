using System.Text.Json.Nodes;
using MadEngine.Core;

namespace MadEditor;

public interface ISerializer
{
    public Type ObjectType { get; }
    
    JsonObject Serialize(MadObject asset);
    MadObject Deserialize(JsonObject asset);
}

public abstract class Serializer<TObject> : ISerializer where TObject : MadObject
{
    public Type ObjectType => typeof(TObject);

    public abstract JsonObject Serialize(TObject obj);

    public abstract TObject Deserialize(JsonObject obj);

    JsonObject ISerializer.Serialize(MadObject obj) => Serialize((TObject)obj);
    MadObject ISerializer.Deserialize(JsonObject obj) => Deserialize(obj);
}
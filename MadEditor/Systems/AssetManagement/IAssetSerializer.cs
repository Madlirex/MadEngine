using System.Text.Json.Nodes;
using MadEngine.Core;

namespace MadEditor;

public interface ISerializer
{
    public Type ObjectType { get; }
    
    JsonObject Serialize(Asset asset);
    Asset Deserialize(JsonObject asset);
}

public abstract class Serializer<TAsset> : ISerializer where TAsset : Asset
{
    public Type AssetType => typeof(TAsset);

    public abstract JsonObject Serialize(TAsset asset);

    public abstract TAsset Deserialize(JsonObject asset);

    JsonObject ISerializer.Serialize(Asset asset) => Serialize((T)asset);
}
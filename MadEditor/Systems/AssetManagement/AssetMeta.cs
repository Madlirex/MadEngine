using System.Text.Json;

namespace MadEditor;

public class AssetMeta
{
    public int Version { get; set; } = 1;
    public string Name { get; set; } = "NewAsset";
    public Guid Guid { get; set; }

    public string Path { get; set; } = "";
    public string Importer { get; set; } = "";

    public Dictionary<string, JsonElement> Settings { get; set; } = [];
}
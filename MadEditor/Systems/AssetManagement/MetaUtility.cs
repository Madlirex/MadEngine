using System.Text.Json;

namespace MadEditor;

public static class MetaUtility
{
    public static AssetMeta Load(string metaPath)
    {
        string json = File.ReadAllText(metaPath);

        return JsonSerializer.Deserialize<AssetMeta>(json, ISerializer.Options)
               ?? throw new InvalidOperationException($"Failed to load meta file '{metaPath}'.");
    }

    public static void Save(string metaPath, AssetMeta meta)
    {
        string json = JsonSerializer.Serialize(meta, ISerializer.Options);

        File.WriteAllText(metaPath, json);
    }
}
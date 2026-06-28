using System.Text.Json;

namespace MadEditor;

public static class MetaUtility
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true
    };

    public static AssetMeta Load(string metaPath)
    {
        string json = File.ReadAllText(metaPath);

        return JsonSerializer.Deserialize<AssetMeta>(json, Options)
               ?? throw new InvalidOperationException($"Failed to load meta file '{metaPath}'.");
    }

    public static void Save(string metaPath, AssetMeta meta)
    {
        string json = JsonSerializer.Serialize(meta, Options);

        File.WriteAllText(metaPath, json);
    }
}
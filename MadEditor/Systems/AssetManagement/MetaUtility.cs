using System.Text.Json;

namespace MadEditor;

public static class MetaUtility
{
    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true
    };

    public static AssetMeta Load(string metaPath)
    {
        string json = File.ReadAllText(metaPath);

        return JsonSerializer.Deserialize<AssetMeta>(json, _options)
               ?? throw new InvalidOperationException($"Failed to load meta file '{metaPath}'.");
    }

    public static void Save(string metaPath, AssetMeta meta)
    {
        string json = JsonSerializer.Serialize(meta, _options);

        File.WriteAllText(metaPath, json);
    }
}
using System.Text.Json;
using MadEngine.Core;

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

    public static AssetMeta GenerateMeta(Asset asset)
    {
        Console.WriteLine(asset.Name + " " + asset.Path);

        bool hasImporter = AssetManager.TryGetImporter(Path.GetExtension(asset.Path), out var importer);
        
        AssetMeta meta = new AssetMeta()
        {
            Guid = asset.Guid,
            Importer = hasImporter ? importer!.Name : "",
            Name = asset.Name,
            Path = asset.Path
        };
        return meta;
    }
}
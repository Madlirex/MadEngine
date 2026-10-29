using System.Text.Json;

namespace MadEngine.Core.AssetManagement;

public class AssetMeta
{
    public static int Version => 1;
    public required Guid Guid;
    public required string Name;
    public required string RelativePath;
    public required string Importer;
    public Dictionary<string, JsonElement> ImportSettings = [];

    public static AssetMeta Generate(Asset asset)
    {
        return new AssetMeta()
        {
            Guid = asset.Guid,
            Name = asset.Name,
            RelativePath = asset.RelativePath,
            Importer = ImporterRegistry.GetImporter(asset.GetType())?.Name ?? "MissingImporter"
        };
    }

    public static void Save(Asset asset)
    {
        Save(asset, asset.AbsolutePath);
    }

    public static void Save(Asset asset, string path)
    {
        AssetMeta meta = Generate(asset);
        File.WriteAllText(path + ".meta", JsonSerializer.Serialize(meta, SerializerSettings.SerializerOptions));
    }

    public static AssetMeta Load(string file)
    {
        return JsonSerializer.Deserialize<AssetMeta>(File.ReadAllText(file), SerializerSettings.SerializerOptions)!;
    }
}
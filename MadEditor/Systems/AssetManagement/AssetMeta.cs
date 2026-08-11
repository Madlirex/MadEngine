using System.Text.Json;
using MadEngine.Core;

namespace MadEditor;

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

    public static AssetMeta Load(string file)
    {
        return JsonSerializer.Deserialize<AssetMeta>(File.ReadAllText(file))!;
    }
}
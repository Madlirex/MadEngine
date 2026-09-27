using System.Text.Json;
using MadEngine.Core;

namespace MadEditor;

public class EditorManifestData
{
    public readonly string Name = "MadEditor";
    public readonly Version Version = new(0, 1, 0);
    public readonly DateTime ReleaseDate = new(2026, 9, 27);
}

public static class EditorManifest
{
    public static readonly EditorManifestData Data = new();
    private static readonly string Path = "manifest.json";

    public static void Save()
    {
        string json = JsonSerializer.Serialize(Data, typeof(EditorManifestData), SerializerSettings.SerializerOptions);
            
        File.WriteAllText(Path, json);
    }
}
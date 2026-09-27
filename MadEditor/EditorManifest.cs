using System.Text.Json;
using MadEngine.Core;

namespace MadEditor;

public class EditorManifestData
{
    public string Name = "MadEditor";
    public string Path = string.Empty;
    public string PathToExecutable = Environment.ProcessPath!;
    public Version Version = new Version(1, 0, 0);
    public DateTime ReleaseDate = new(2026, 9, 27);
    public bool IsLts;
    public List<string> Platforms = [];
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
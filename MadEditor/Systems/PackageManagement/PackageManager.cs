using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MadEditor.PackageManagement;

public static class PackageManager
{
    public static IReadOnlyList<PackageMeta> PackageMetas => _packageMetas;
    private static List<PackageMeta> _packageMetas = [];

    public static IReadOnlyList<string> PackagePaths => _packagePaths;
    private static List<string> _packagePaths = [];
    
    private static string _packagesListFile = "packages.json";
    
    public static void LoadPackageMetas()
    {
        if (!File.Exists(_packagesListFile)) return;
        
        string data = File.ReadAllText(_packagesListFile);
        JsonNode? json = JsonNode.Parse(data);
        if (json is not JsonArray jsonArray) return;

        foreach (JsonNode? jsonNode in jsonArray)
        {
            if(jsonNode == null) continue;
            LoadPackageMeta(jsonNode.GetValue<string>());
        }
    }

    public static void LoadPackageMeta(string packagePath)
    {
        if (!File.Exists(packagePath)) return;
        
        JsonNode? json = JsonNode.Parse(File.ReadAllText(packagePath));

        PackageMeta? meta = json.Deserialize<PackageMeta>(SerializerSettings.SerializerOptions);
        if (meta != null) _packageMetas.Add(meta);
    }

    public static void SavePackageMetas()
    {
        string data = JsonSerializer.Serialize(_packagePaths, SerializerSettings.SerializerOptions);
        File.WriteAllText(_packagesListFile, data);
    }
}
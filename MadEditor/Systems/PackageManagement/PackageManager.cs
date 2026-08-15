using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using MadEngine.Core;

namespace MadEditor.PackageManagement;

public static class PackageManager
{
    private static readonly Dictionary<Guid, Package> Packages = [];
    
    private static readonly Dictionary<Guid, PackageMeta> NewerPackagesMetas = [];
    
    public static IReadOnlyDictionary<Guid, PackageMeta> PackagesMetas => PackageMetas;
    private static readonly Dictionary<Guid, PackageMeta> PackageMetas = [];
    
    private static readonly List<string> PackagePaths = [];

    private const string PackagesListFile = "packages.json";

    public static void LoadPackageMetas()
    {
        if (!File.Exists(PackagesListFile)) return;
        
        string data = File.ReadAllText(PackagesListFile);
        JsonNode? json = JsonNode.Parse(data);
        if (json is not JsonArray jsonArray) return;

        foreach (string path in Directory.GetFiles(Application.PackagesPath, "*", SearchOption.TopDirectoryOnly))
            LoadPackageMeta(path + @"\packages.pck");
        
        foreach (JsonNode? jsonNode in jsonArray)
        {
            if(jsonNode == null) continue;
            LoadPackageMeta(jsonNode.GetValue<string>());
        }
        foreach(PackageMeta meta in PackageMetas.Values)
        {
            Console.WriteLine(meta.Guid);
        }
    }

    public static void LoadPackageMeta(string packagePath)
    {
        if (!File.Exists(packagePath)) return;
        
        JsonNode? json = JsonNode.Parse(File.ReadAllText(packagePath));

        PackageMeta? meta = json.Deserialize<PackageMeta>(SerializerSettings.SerializerOptions);
        if (meta == null) return;

        if (PackageMetas.TryAdd(meta.Guid, meta))
        {
            return;
        }

        if (meta.Version <= PackageMetas[meta.Guid].Version) return;
        if(!NewerPackagesMetas.TryAdd(meta.Guid, meta)) NewerPackagesMetas[meta.Guid] = meta;
    }

    public static void SavePackageMetas()
    {
        string data = JsonSerializer.Serialize(PackagePaths, SerializerSettings.SerializerOptions);
        File.WriteAllText(PackagesListFile, data);
    }

    public static void LoadPackages()
    {
        if(!Directory.Exists(Application.PackagesPath)) Directory.CreateDirectory(Application.PackagesPath);
        
        AssetManager.InitializeAssets(Application.PackagesPath);
        AssetManager.LoadAssets(Application.PackagesPath);
    }
}
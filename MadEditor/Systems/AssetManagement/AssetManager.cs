using MadEngine.Core;

namespace MadEditor;

public static class AssetManager
{
    public static string ProjectPath => _projectPath;
    public static string AssetsPath => _projectPath + "/Assets";
    private static string _projectPath = "";

    public static void RecompileScripts()
    {
        if (!Directory.Exists(ProjectPath)) return;
        var scriptFiles = Directory.GetFiles(ProjectPath, "*.cs", SearchOption.AllDirectories);

        ScriptDomain.ReloadFromFiles(scriptFiles);
    }
    
    public static void SetProjectPath(string path)
    {
        _projectPath = path;
    }
    
    public static void LoadProject(string path)
    {
        InitializeAssets(path);
        foreach(var pair in AssetRegistry.ObjectMap)
            Console.WriteLine($"{pair.Key}: {pair.Value.Name}");
        LoadAssets(path);
    }

    public static void SaveProject(Asset[] assets)
    {
        SaveAssets(assets);
    }
    
    public static void InitializeAssets(string path)
    {
        foreach (string file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
        {
            InitializeAsset(file);
        }
    }

    public static void InitializeAsset(string file)
    {
        if (File.Exists(file + ".meta"))
        {
            AssetMeta meta = AssetMeta.Load(file + ".meta");
            var importer = ImporterRegistry.GetImporter(meta.Importer);
            importer?.Initialize(meta);
        }
        else
        {
            var importer = ImporterRegistry.GetImporterByExtension(Path.GetExtension(file));
            importer?.Initialize(file);
        }
    }

    public static void LoadAssets(string path)
    {
        foreach (string file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
        {
            LoadAsset(file);
        }
    }

    public static void LoadAsset(string file)
    {
        var importer = ImporterRegistry.GetImporterByExtension(Path.GetExtension(file));
        importer?.Import(file);
    }

    public static void SaveAssets(Asset[] assets)
    {
        foreach (Asset asset in assets)
        {
            SaveAsset(asset);
        }
    }

    public static void SaveAsset(Asset asset)
    {
        var importer = ImporterRegistry.GetImporter(asset.GetType());
        importer?.Save(asset);
        AssetMeta.Save(asset);
    }
}
namespace MadEditor;

public static class AssetManager
{
    public static string ProjectPath => _projectPath;
    public static string AssetsPath => _projectPath + @"\Assets\";
    public static string PackagesPath => _projectPath + @"\Packages\";
    private static string _projectPath = "";

    private static Dictionary<string, IAssetImporter> _importers = new();
    private static Dictionary<string, IAssetImporter> _importersByExtension = new();

    public static void SetPath(string path)
    {
        _projectPath = path;
    }
    
    public static void LoadProject(string path)
    {
        LoadAssets(path);
    }

    public static void SaveProject()
    {
        SaveAssets();
    }
    
    public static void RecompileScripts()
    {
        if (Directory.Exists(ProjectPath))
        {
            var scriptFiles = Directory.GetFiles(ProjectPath, "*.cs", SearchOption.AllDirectories);

            ScriptDomain.ReloadFromFiles(scriptFiles);
        }
    }

    public static void LoadAssets(string path)
    {
        DiscoverImporters();
        
        if (Directory.Exists(path))
        {
            var files = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                LoadAsset(file);
            }
        }
    }
    
    public static void LoadAsset(string path)
    {
        IAssetImporter? importer;
        Asset asset;
        if (!File.Exists(path + ".meta"))
        {
            if (_importersByExtension.TryGetValue(Path.GetExtension(path), out importer))
            {
                asset = importer.Load(path);
            }
            else
            {
                return;
            }
        }
        else
        {
            AssetMeta meta = MetaUtility.Load(path + ".meta");
            importer = _importers[meta.Importer];

            asset = importer.Load(meta);
        }
        
        AssetRegistry.RegisterAsset(asset, path);
    }

    public static void SaveAssets()
    {
        foreach (var asset in AssetRegistry.Assets)
        {
            SaveAsset(asset, AssetRegistry.GetPath(asset.Guid));
        }
    }

    public static void SaveAsset(Asset asset, string path)
    {
        _importersByExtension[Path.GetExtension(path)].Save(asset, path);
        
        AssetMeta meta = new AssetMeta()
        {
            Guid = asset.Guid,
            Importer = _importersByExtension[Path.GetExtension(path)].Name,
            Name = asset.Name,
            Path = path
        };
        MetaUtility.Save(path + ".meta", meta);
    }

    public static void DiscoverImporters()
    {
        _importers.Clear();
        _importersByExtension.Clear();
        
        var importers = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IAssetImporter).IsAssignableFrom(type) && type is { IsAbstract: false, IsInterface: false });
        foreach(Type importer in importers)
        {
            IAssetImporter importerInstance = (IAssetImporter)Activator.CreateInstance(importer)!;
            
            _importers.Add(importerInstance.Name, importerInstance);
            
            foreach (string extension in importerInstance.Extensions)
            {
                _importersByExtension.Add(extension, importerInstance);
            }
        }
    }
}
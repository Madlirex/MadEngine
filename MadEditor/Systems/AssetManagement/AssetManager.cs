namespace MadEditor;

public static class AssetManager
{
    public static string ProjectPath => _projectPath;
    public static string AssetsPath => _projectPath + @"\Assets\";
    public static string PackagesPath => _projectPath + @"\Packages\";
    private static string _projectPath = "";
    
    private static Dictionary<string, Guid> _guidByPath = new();
    private static Dictionary<Guid, string> _pathByGuid = new();
    private static Dictionary<Guid, Asset> _assets = new();
    private static Dictionary<Type, List<Asset>> _assetRegistries = new();

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

    public static Asset GetAsset(Guid guid)
    {
        return _assets[guid];
    }
    
    public static Guid GetGuid(string path)
    {
        return _guidByPath[path];
    }

    public static string GetPath(Guid guid)
    {
        return _pathByGuid[guid];
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
        
        _guidByPath.Add(path, asset.Guid);
        _pathByGuid.Add(asset.Guid, path);
        _assets.Add(asset.Guid, asset);
        RegisterAsset(asset);
    }

    public static void SaveAssets()
    {
        foreach (var asset in _assets)
        {
            SaveAsset(asset.Value, _pathByGuid[asset.Key]);
        }
    }

    public static void SaveAsset(Asset asset, string path)
    {
        AssetMeta meta = new AssetMeta()
        {
            Guid = asset.Guid,
            Importer = _importersByExtension[Path.GetExtension(path)].Name,
            Name = asset.Name,
            Path = path
        };
        MetaUtility.Save(path + ".meta", meta);
    }

    public static void UnloadAsset(Asset asset)
    {
        string path = GetPath(asset.Guid);
        _guidByPath.Remove(path);
        _pathByGuid.Remove(asset.Guid);
        _assets.Remove(asset.Guid);
        UnregisterAsset(asset);
    }

    public static void RegisterAsset(Asset asset)
    {
        if (!_assetRegistries.TryGetValue(asset.AssetType, out List<Asset>? value))
        {
            value = [];
            _assetRegistries[asset.AssetType] = value;
        }

        value.Add(asset);
    }

    public static void UnregisterAsset(Asset asset)
    {
        _assetRegistries[asset.AssetType].Remove(asset);
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
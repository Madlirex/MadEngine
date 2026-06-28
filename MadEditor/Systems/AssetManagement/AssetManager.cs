using MadEngine.Core;
using MadEngine.Core.SceneManagement;

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
        DiscoverImporters();
        SerializerRegistry.DiscoverSerializers();
        InstantiateAssets(path);
        LoadAssets(AssetRegistry.Assets);
        Console.WriteLine(SceneManager.Scenes.Count);
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
    
    public static void InstantiateAssets(string path)
    {
        if (Directory.Exists(path))
        {
            var files = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                InstantiateAsset(file);
            }
        }
    }

    public static void InstantiateAsset(string path)
    {
        Asset asset;
        if (File.Exists(path + ".meta"))
        {
            AssetMeta meta = MetaUtility.Load(path + ".meta");
            asset = _importers[meta.Importer].Instantiate(meta);
            asset.Path = path;
        }
        else
        {
            if (_importersByExtension.TryGetValue(Path.GetExtension(path), out var importer))
            {
                asset = importer.Instantiate(path);
                asset.Path = path;
            }
            else
            {
                return;
            }
        }
        
        AssetRegistry.RegisterAsset(asset, path);
    }

    public static void LoadAssets(Asset[] assets)
    {
        Console.WriteLine(assets.Length);
        foreach (Asset asset in assets)
        {
            LoadAsset(asset);
        }
    }
    
    public static void LoadAsset(Asset asset)
    {
        Console.WriteLine(asset.Path);
        if (_importersByExtension.TryGetValue(Path.GetExtension(asset.Path), out var importer))
        {
            importer.Load(asset);
        }
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

        AssetMeta meta = MetaUtility.GenerateMeta(asset);
        
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

    public static bool TryGetImporter(string extension, out IAssetImporter? importer)
    {
        return _importersByExtension.TryGetValue(extension, out importer);
    }
}
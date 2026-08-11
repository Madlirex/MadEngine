using MadEngine.Core;
using OpenTK.Graphics.ES11;

namespace MadEditor;

public static class AssetManager
{
    public static void LoadProject(string path)
    {
        InitializeAssets(path);
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
            ImporterRegistry.GetImporter(meta.Importer)!.Initialize(meta);
        }
        else
        {
            ImporterRegistry.GetImporterByExtension(Path.GetExtension(file))!.Initialize(file);
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
        ImporterRegistry.GetImporterByExtension(Path.GetExtension(file))!.Import(file);
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
        ImporterRegistry.GetImporter(asset.GetType())!.Save(asset);
    }
}
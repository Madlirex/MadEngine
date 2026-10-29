using MadEngine.Core;
using MadEngine.Core.AssetManagement;

namespace MadEditor;

public static class BuildManager
{
    public static void ExportGame(string outputDirectory, string gameName, byte[] runtimeDllBytes, string resourceDir)
    {
        string runtimeTemplateSourceDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "runtime");

        if (!Directory.Exists(runtimeTemplateSourceDir))
        {
            throw new DirectoryNotFoundException($"Critical Failure: Runtime template directory missing at {runtimeTemplateSourceDir}. Did the MSBuild task fail?");
        }

        Directory.CreateDirectory(outputDirectory);
        
        foreach (var file in Directory.GetFiles(runtimeTemplateSourceDir))
        {
            string fileName = Path.GetFileName(file);
            string destFile = Path.Combine(outputDirectory, fileName);
            
            if (fileName == "MadEngine.Runtime.exe")
                destFile = Path.Combine(outputDirectory, $"{gameName}.exe");
            else if (fileName == "MadEngine.Runtime.runtimeconfig.json")
                destFile = Path.Combine(outputDirectory, $"{gameName}.runtimeconfig.json");

            File.Copy(file, destFile, true);
        }
        
        string targetScriptPath = Path.Combine(outputDirectory, "GameScripts.Runtime.dll");
        File.WriteAllBytes(targetScriptPath, runtimeDllBytes);
        
        PackAssets(resourceDir, Path.Combine(outputDirectory, "Resources"));
    }

    public static void PackAssets(string resourceDir, string outputDirectory)
    {
        foreach (string path in Directory.GetFiles(resourceDir, "*.*", SearchOption.AllDirectories))
        {
            if(AssetRegistry.GetGuid(path) is { } guid)
            {
                Asset? asset = AssetRegistry.GetAsset(guid);
                
                if (asset is null) continue;
                
                PackAsset(asset, Path.Combine(outputDirectory, asset.RelativePath, SerializerSettings.PackedExtension));            
            }
            else
            {
                CopyFile(path, Path.Combine(outputDirectory, Path.GetRelativePath(resourceDir, path)));
                Console.WriteLine(Path.GetRelativePath(resourceDir, path));
            }
        }
    }
    
    public static void PackAsset(Asset asset, string outputPath)
    {
        string? targetDirectory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory);
        }
        
        if (File.Exists(outputPath))
        {
            DateTime sourceTime = File.GetLastWriteTimeUtc(asset.AbsolutePath);
            DateTime targetTime = File.GetLastWriteTimeUtc(outputPath);

            if (sourceTime <= targetTime) return;
        }

        var importer = ImporterRegistry.GetImporter(asset.GetType());
        if (importer is { } imp)
        {
            importer.Pack(asset, outputPath);
        }
        AssetMeta.Save(asset, outputPath);
    }

    public static void CopyFile(string targetPath, string outputPath)
    {
        string? targetDirectory = Path.GetDirectoryName(outputPath);
        if (!string.IsNullOrEmpty(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory);
        }
        
        if (File.Exists(outputPath))
        {
            DateTime sourceTime = File.GetLastWriteTimeUtc(targetPath);
            DateTime targetTime = File.GetLastWriteTimeUtc(outputPath);

            if (sourceTime <= targetTime) return;
        }
        
        File.Copy(targetPath, outputPath, true);
    }
}
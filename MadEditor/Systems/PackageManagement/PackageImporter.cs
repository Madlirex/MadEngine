using MadEngine.Core;

namespace MadEditor.PackageManagement;

public static class PackageImporter
{
    public static void ImportPackage(string packagePath, PackageMeta packageMeta)
    {
        CopyDirectory(packagePath, Application.PackagesPath + @$"\{packageMeta.Name}\");
    }

    public static void RemovePackage(PackageMeta packageMeta)
    {
        string targetFilePath = Path.Combine(Application.PackagesPath, packageMeta.Name);
        if (!Directory.Exists(targetFilePath)) return;
        
        Directory.Delete(targetFilePath, true);
    }
    
    public static void CopyDirectory(string sourceDir, string destinationDir, bool overwrite = true)
    {
        var dir = new DirectoryInfo(sourceDir);

        if (!dir.Exists)
        {
            throw new DirectoryNotFoundException($"Source directory does not exist: {sourceDir}");
        }
        
        Directory.CreateDirectory(destinationDir);
        
        foreach (FileInfo file in dir.GetFiles())
        {
            string targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath, overwrite);
        }
        
        foreach (DirectoryInfo subDir in dir.GetDirectories())
        {
            string targetSubDirPath = Path.Combine(destinationDir, subDir.Name);
            
            CopyDirectory(subDir.FullName, targetSubDirPath, overwrite);
        }
    }
}
using System.Text.Json;
using MadEditor.PackageManagement;
using MadEngine.Core;

namespace MadEditor;

public static class ProjectManager
{
    public static string ProjectPath => _projectPath;
    private static string _projectPath = "";
    
    public static ProjectInfo ProjectInfo => ProjectSettingsRegistry.GetSettings<ProjectInfo>() ?? ProjectSettingsRegistry.CreateSettings<ProjectInfo>();
    
    public static void LoadProject()
    {
        ProjectSettingsManager.LoadAllSettings();
        
        PackageManager.LoadPackages();
        AssetManager.LoadProject();
    }

    public static void SaveProject()
    {
        ProjectSettingsManager.SaveAllSettings();
        
        PackageManager.SavePackageMetas();
        AssetManager.SaveProject(AssetRegistry.Assets);
    }
    
    public static void SetProjectPath(string path)
    {
        _projectPath = path;
    }
}
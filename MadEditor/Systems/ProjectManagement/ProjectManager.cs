using System.Text.Json;
using MadEditor.PackageManagement;
using MadEngine.Core;

namespace MadEditor;

public static class ProjectManager
{
    public static string ProjectPath => _projectPath;
    private static string _projectPath = "";

    public static string ProjectInfoPath => Path.Combine(ProjectPath, "project.madx");

    public static ProjectInfo ProjectInfo => _projectInfo;
    private static ProjectInfo _projectInfo = new();
    
    public static void LoadProject()
    {
        LoadProjectInfo();
        
        PackageManager.LoadPackages();
        AssetManager.LoadProject();
    }

    public static void SaveProject()
    {
        SaveProjectInfo();
        
        PackageManager.SavePackageMetas();
        AssetManager.SaveProject(AssetRegistry.Assets);
    }

    private static void LoadProjectInfo()
    {
        if (!File.Exists(ProjectInfoPath))
        {
            Debug.LogError($"No project file found at: {ProjectInfoPath}");
            return;
        }

        try
        {
            string jsonString = File.ReadAllText(ProjectInfoPath);
            _projectInfo = JsonSerializer.Deserialize<ProjectInfo>(jsonString) ?? new ProjectInfo();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load project info: {ex.Message}");
        }
    }

    private static void SaveProjectInfo()
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(_projectInfo, SerializerSettings.SerializerOptions);
            File.WriteAllText(ProjectInfoPath, jsonString);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save project info: {ex.Message}");
        }
    }
    
    public static void SetProjectPath(string path)
    {
        _projectPath = path;
    }
}
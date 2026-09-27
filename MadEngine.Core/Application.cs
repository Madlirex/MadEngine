namespace MadEngine.Core;

public static class Application
{
    private static ProjectInfo _projectInfo => ProjectSettingsRegistry.GetSettings<ProjectInfo>() ?? new ProjectInfo();
    
    public static string PersistentDataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), _projectInfo.Company, _projectInfo.Name);
    public static string Directory = string.Empty;
    public static string AssetsPath => Directory + @"\Assets\";
    public static string PackagesPath => Directory + @"\Packages\";
    public static string ConfigPath => Directory + @"\Config\";
    public static string ProjectName => Path.GetFileNameWithoutExtension(Directory);
}
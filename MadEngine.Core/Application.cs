namespace MadEngine.Core;

public static class Application
{
    private static ProjectInfo ProjectInfo => ProjectSettingsRegistry.GetSettings<ProjectInfo>() ?? ProjectSettingsRegistry.CreateSettings<ProjectInfo>();
    
    public static string PersistentDataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ProjectInfo.Company, ProjectInfo.Name);
    public static string Directory = string.Empty;
    public static string AssetsPath => Directory + @"\Assets\";
    public static string PackagesPath => Directory + @"\Packages\";
    public static string ConfigPath => Directory + @"\Config\";
    public static string ProjectName => Path.GetFileNameWithoutExtension(Directory);
}
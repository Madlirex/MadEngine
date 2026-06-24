namespace MadEditor;

public static class AssetManager
{
    public static string ProjectPath => _projectPath;
    public static string AssetsPath => _projectPath + "Assets/";
    public static string PackagesPath => _projectPath + "Packages/";
    private static string _projectPath = "";

    public static void LoadProject(string path)
    {
        _projectPath = path;
    }

    public static void RecompileScripts()
    {
        if (Directory.Exists(ProjectPath))
        {
            var scriptFiles = Directory.GetFiles(ProjectPath, "*.cs", SearchOption.AllDirectories);

            ScriptDomain.ReloadFromFiles(scriptFiles);
        }
    }
}
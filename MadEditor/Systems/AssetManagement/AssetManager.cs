namespace MadEditor;

public static class AssetManager
{
    public static string ProjectPath => _projectPath;
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
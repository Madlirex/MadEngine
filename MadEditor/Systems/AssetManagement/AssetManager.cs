namespace MadEditor;

public static class AssetManager
{
    public static string ProjectPath => _projectPath;
    private static string _projectPath = "";

    public static void LoadProject(string path)
    {
        _projectPath = path;
    }
}
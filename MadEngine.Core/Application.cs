namespace MadEngine.Core;

public static class Application
{
    public static string DataPath = "";
    public static string Directory = "";
    public static string AssetsPath => Directory + @"\Assets\";
    public static string PackagesPath => Directory + @"\Packages\";
    public static string ProjectName => Path.GetFileNameWithoutExtension(Directory);
}
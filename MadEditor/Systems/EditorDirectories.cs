namespace MadEditor;

public static class EditorDirectories
{
    public static string ExecutablePath { get; } = AppContext.BaseDirectory;
    
    private static string DataPathBase { get; } = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    public static string PersistentDataPath { get; } = Path.Combine(DataPathBase, "MadlirexStudios", "MadEditor");

    public static string ConfigPath { get; } = Path.Combine(PersistentDataPath, "Config");
}
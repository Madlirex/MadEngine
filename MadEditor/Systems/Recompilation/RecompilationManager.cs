namespace MadEditor;

public static class RecompilationManager
{
    public static void RecompileScripts(bool performSnapshot = true)
    {
        Console.WriteLine("Recompiling scripts...");
        if (!Directory.Exists(ProjectManager.ProjectPath)) return;
        var scriptFiles = Directory.GetFiles(ProjectManager.ProjectPath, "*.cs", SearchOption.AllDirectories);
        
        if(performSnapshot) ScriptDomain.ReloadDomain(scriptFiles);
        else ScriptDomain.Compile(scriptFiles);
    }
}
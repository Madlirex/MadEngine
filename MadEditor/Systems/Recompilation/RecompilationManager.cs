namespace MadEditor;

public static class RecompilationManager
{
    public static void RecompileScripts(bool performSnapshot = true)
    {
        Console.WriteLine("Recompiling scripts...");
        if (!Directory.Exists(ProjectManager.ProjectPath)) return;
        var scriptFiles = ProjectManager.GetAllFiles("*.cs");
        
        if(performSnapshot) ScriptDomain.ReloadDomain(scriptFiles);
        else ScriptDomain.Compile(scriptFiles);
    }
}
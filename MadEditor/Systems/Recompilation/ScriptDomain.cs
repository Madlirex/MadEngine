using System.Reflection;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;

namespace MadEditor;

public static class ScriptDomain
{
    public static Assembly? RuntimeAssembly { get; private set; }
    public static Assembly? EditorAssembly { get; private set; }
    public static ScriptLoadContext? CurrentContext { get; private set; }
    
    public static void ReloadDomain(string[] sourceFiles)
    {
        Scene activeScene = SceneManager.ActiveScene;
        string activeScenePath = activeScene.AbsolutePath;
    
        SceneSnapshotController.TakeSnapshot(activeScene);
    
        Compile(sourceFiles);
        AssetRegistry.Clear(); 
        RegistryBootstrapper.ReinitializeAll();

        AssetManager.PathsToSkip.Add(activeScenePath);
    
        ProjectManager.LoadProject();

        AssetManager.PathsToSkip.Clear();
        
        Scene restoredScene = SceneSnapshotController.RestoreSnapshot();
        SceneManager.LoadScene(restoredScene);
    }

    public static void Compile(string[] sourceFiles)
    {
        ReloadFromFiles(sourceFiles);
    }

    private static void ReloadFromFiles(string[] sourceFiles)
    {
        var (runtimeDll, editorDll) = ScriptCompiler.CompileProject(sourceFiles);

        if (runtimeDll == null)
        {
            Console.WriteLine("Script compilation failed.");
            return;
        }

        Load(runtimeDll, editorDll);
    }
    
    private static void Load(byte[] runtimeDll, byte[]? editorDll)
    {
        Unload();

        var context = new ScriptLoadContext();
        CurrentContext = context;
        AssemblyProvider.ClearAssemblies();
        
        AddCoreEngineAssemblies();
        
        using (var ms = new MemoryStream(runtimeDll))
        {
            RuntimeAssembly = context.LoadFromStream(ms);
            AssemblyProvider.AddAssembly(RuntimeAssembly);
        }
        
        if (editorDll is not { Length: > 0 }) return;
        
        using (var ms = new MemoryStream(editorDll))
        {
            EditorAssembly = context.LoadFromStream(ms);
            AssemblyProvider.AddAssembly(EditorAssembly);
        }
    }
    
    private static void AddCoreEngineAssemblies()
    {
        Assembly coreAssembly = typeof(MadObject).Assembly;
        Assembly editorAssembly = typeof(ScriptDomain).Assembly;

        if (!AssemblyProvider.Assemblies.Contains(coreAssembly))
        {
            AssemblyProvider.AddAssembly(coreAssembly);
        }

        if (!AssemblyProvider.Assemblies.Contains(editorAssembly))
        {
            AssemblyProvider.AddAssembly(editorAssembly);
        }
    }
    
    private static void Unload()
    {
        AssemblyProvider.ClearAssemblies();

        var context = CurrentContext;

        RuntimeAssembly = null;
        EditorAssembly = null;
        CurrentContext = null;

        if (context == null) return;
        FieldDrawingManager.OnSelectionChanged(null);

        context.Unload();
            
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
    }
}

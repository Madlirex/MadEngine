using System.Reflection;

namespace MadEditor;

public static class ScriptDomain
{
    private static readonly List<Assembly> Assemblies = new();

    public static Assembly? RuntimeAssembly { get; private set; }
    public static Assembly? EditorAssembly { get; private set; }
    public static ScriptLoadContext? CurrentContext { get; private set; }

    public static Type? GetType(string typeName)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .Select(assembly => assembly.GetType(typeName.Split(',')[0].Trim()))
            .FirstOrDefault(type => type != null);
    }

    public static void Compile(string[] sourceFiles)
    {
        ReloadFromFiles(sourceFiles);
        RegistryBootstrapper.ReinitializeAll();
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
        Assemblies.Clear();
        
        using (var ms = new MemoryStream(runtimeDll))
        {
            RuntimeAssembly = context.LoadFromStream(ms);
            Assemblies.Add(RuntimeAssembly);
        }
        
        if (editorDll is not { Length: > 0 }) return;
        {
            using var ms = new MemoryStream(editorDll);
            EditorAssembly = context.LoadFromStream(ms);
            Assemblies.Add(EditorAssembly);
        }
    }

    private static void Unload()
    {
        Assemblies.Clear();

        var context = CurrentContext;

        RuntimeAssembly = null;
        EditorAssembly = null;
        CurrentContext = null;

        if (context != null)
        {
            context.Unload();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
    
    public static Type[] GetAllTypes()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .ToArray();
    }

    public static Type[] GetTypesImplementing(Type baseType)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t =>
                t is { IsClass: true, IsAbstract: false } &&
                baseType.IsAssignableFrom(t))
            .ToArray();
    }

    public static Type[] GetTypesWithName(string name)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.Name == name)
            .ToArray();
    }
}
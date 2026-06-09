using System.Reflection;

namespace MadEditor;

public static class ScriptDomain
{
    private static readonly List<Assembly> _assemblies = new();

    public static Assembly? CurrentAssembly { get; private set; }
    public static ScriptLoadContext? CurrentContext { get; private set; }


    public static void ReloadFromFiles(string[] sourceFiles)
    {
        var dll = ScriptCompiler.CompileToBytes(sourceFiles);

        if (dll == null)
        {
            Console.WriteLine("Script compilation failed.");
            return;
        }

        Load(dll);
    }
    
    public static void Load(byte[] dllBytes)
    {
        Unload();

        var context = new ScriptLoadContext();

        using var ms = new MemoryStream(dllBytes);
        var assembly = context.LoadFromStream(ms);

        CurrentContext = context;
        CurrentAssembly = assembly;

        _assemblies.Clear();
        _assemblies.Add(assembly);
    }

    public static void Unload()
    {
        _assemblies.Clear();

        var context = CurrentContext;

        CurrentAssembly = null;
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
        return _assemblies
            .SelectMany(a => a.GetTypes())
            .ToArray();
    }

    public static Type[] GetComponentTypes(Type componentBaseType)
    {
        return _assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t =>
                t is { IsClass: true, IsAbstract: false } &&
                componentBaseType.IsAssignableFrom(t))
            .ToArray();
    }

    public static Type[] GetTypesWithName(string name)
    {
        return _assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t.Name == name)
            .ToArray();
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;

namespace MadEditor;

public static class ScriptDomain
{
    private static readonly List<Assembly> Assemblies = new();

    public static Assembly? RuntimeAssembly { get; private set; }
    public static Assembly? EditorAssembly { get; private set; }
    public static ScriptLoadContext? CurrentContext { get; private set; }

    // Stores a reference to the old context to analyze it if it leaks
    private static WeakReference? _zombieContextRef;

    public static Type? GetType(string typeName)
    {
        // FIX: Look inside active tracked assemblies to prevent zombie duplication lookups
        foreach (var assembly in Assemblies)
        {
            var type = assembly.GetType(typeName.Split(',')[0].Trim());
            if (type != null) return type;
        }
        return null;
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
        
        AddCoreEngineAssemblies();
        
        using (var ms = new MemoryStream(runtimeDll))
        {
            RuntimeAssembly = context.LoadFromStream(ms);
            Assemblies.Add(RuntimeAssembly);
        }
        
        if (editorDll is not { Length: > 0 }) return;
        
        using (var ms = new MemoryStream(editorDll))
        {
            EditorAssembly = context.LoadFromStream(ms);
            Assemblies.Add(EditorAssembly);
        }
    }
    
    private static void AddCoreEngineAssemblies()
    {
        var runningAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        
        foreach (var assembly in runningAssemblies)
        {
            string? name = assembly.GetName().Name;
            if (name == "MadEngine" || name == "MadEngine.Core" || name == "MadEditor")
            {
                if (!Assemblies.Contains(assembly))
                {
                    Assemblies.Add(assembly);
                }
            }
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
            FieldDrawingManager.OnSelectionChanged(null);

            context.Unload();
            
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }
    
    public static Type[] GetAllTypes()
    {
        return Assemblies.SelectMany(a => a.GetTypes()).ToArray();
    }

    public static Type[] GetTypesImplementing(Type baseType)
    {
        return Assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t is { IsClass: true, IsAbstract: false } && baseType.IsAssignableFrom(t))
            .ToArray();
    }

    public static Type[] GetTypesWithName(string name)
    {
        return Assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t.Name == name)
            .ToArray();
    }
}

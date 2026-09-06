using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using MadEditor.PackageManagement;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;

namespace MadEditor;

public static class ScriptDomain
{
    public static IReadOnlyList<Assembly> Assemblies => _assemblies;
    private static List<Assembly> _assemblies = [];

    public static Assembly? RuntimeAssembly { get; private set; }
    public static Assembly? EditorAssembly { get; private set; }
    public static ScriptLoadContext? CurrentContext { get; private set; }
    
    private static WeakReference? _zombieContextRef;

    public static Type? GetType(string typeName)
    {
        foreach (var assembly in Assemblies)
        {
            var type = assembly.GetType(typeName.Split(',')[0].Trim());
            if (type != null) return type;
        }
        return null;
    }
    
    public static void ReloadDomain(string[] sourceFiles)
    {
        int index = SceneManager.Scenes.IndexOf(SceneManager.ActiveScene);
        
        Compile(sourceFiles);
        
        AssetRegistry.Clear(); 
        
        RegistryBootstrapper.ReinitializeAll();
        
        PackageManager.LoadPackages();
        AssetManager.LoadProject();
        
        SceneManager.LoadScene(index);
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
        _assemblies.Clear();
        
        AddCoreEngineAssemblies();
        
        using (var ms = new MemoryStream(runtimeDll))
        {
            RuntimeAssembly = context.LoadFromStream(ms);
            _assemblies.Add(RuntimeAssembly);
        }
        
        if (editorDll is not { Length: > 0 }) return;
        
        using (var ms = new MemoryStream(editorDll))
        {
            EditorAssembly = context.LoadFromStream(ms);
            _assemblies.Add(EditorAssembly);
        }
    }
    
    private static void AddCoreEngineAssemblies()
    {
        Assembly coreAssembly = typeof(MadObject).Assembly;
        Assembly editorAssembly = typeof(ScriptDomain).Assembly;

        if (!_assemblies.Contains(coreAssembly))
        {
            _assemblies.Add(coreAssembly);
        }

        if (!_assemblies.Contains(editorAssembly))
        {
            _assemblies.Add(editorAssembly);
        }
    }
    
    private static void Unload()
    {
        _assemblies.Clear();

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

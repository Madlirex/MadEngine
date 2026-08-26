using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

namespace MadEditor;

public static class ScriptCompiler
{
    public static string RuntimeAssemblyName = "GameScripts.Runtime";
    public static string EditorAssemblyName = "GameScripts.Editor";
    
    public static (byte[]? runtimeDll, byte[]? editorDll) CompileProject(string[] allSourceFiles)
    {
        var editorFiles = allSourceFiles
            .Where(f => f.Split(Path.DirectorySeparatorChar).Contains("Editor"))
            .ToArray();
        var runtimeFiles = allSourceFiles.Except(editorFiles).ToArray();
        
        var runtimeBytes = CompileToBytes(runtimeFiles, RuntimeAssemblyName, null);
        if (runtimeBytes == null) return (null, null);
        
        var editorBytes = CompileToBytes(editorFiles, EditorAssemblyName, runtimeBytes);

        return (runtimeBytes, editorBytes);
    }
    
    private static byte[]? CompileToBytes(string[] sourceFiles, string assemblyName, byte[]? runtimeDependency)
    {
        if (sourceFiles.Length == 0) return [];

        var syntaxTrees = sourceFiles
            .Select(f => CSharpSyntaxTree.ParseText(File.ReadAllText(f)))
            .ToArray();
        
        var references = GetReferences();
        Console.WriteLine(
            references.Any(r =>
                r.Display?.Contains("System.Console.dll") == true)
        );
        if (runtimeDependency != null)
        {
            references.Add(MetadataReference.CreateFromImage(runtimeDependency));
        }
        
        var compilation = CSharpCompilation.Create(
            assemblyName,
            syntaxTrees,
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );
        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);

        if (result.Success) return ms.ToArray();
        foreach (var d in result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error))
            Console.WriteLine(d);

        return null;
    }

    public static Assembly LoadAssembly(byte[] dllBytes, out ScriptLoadContext context)
    {
        context = new ScriptLoadContext();

        using var ms = new MemoryStream(dllBytes);
        return context.LoadFromStream(ms);
    }
    
    private static List<PortableExecutableReference> GetReferences()
    {
        if (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") is not string trustedAssemblies)
            throw new InvalidOperationException("Could not obtain trusted platform assemblies.");

        var references = trustedAssemblies
            .Split(Path.PathSeparator)
            .Select(path => MetadataReference.CreateFromFile(path))
            .ToList();
        
        return references;
    }
}
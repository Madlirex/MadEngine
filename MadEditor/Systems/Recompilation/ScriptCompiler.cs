using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;
using MadEngine.Core;

namespace MadEditor;

public static class ScriptCompiler
{
    private static readonly string GlobalUsingsSource = """

                                                                global using System;
                                                                global using System.IO;
                                                                global using System.Linq;
                                                                global using System.Collections.Generic;
                                                                global using MadEngine;
                                                                global using MadEngine.Core;
                                                            
                                                        """;
    
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

        var treesList = sourceFiles
            .Select(f => CSharpSyntaxTree.ParseText(File.ReadAllText(f)))
            .ToList();

        treesList.Add(CSharpSyntaxTree.ParseText(GlobalUsingsSource));
        
        var syntaxTrees = treesList.ToArray();
        
        var references = GetReferences();
        
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
        
        foreach (var d in result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error))
        {
            var lineSpan = d.Location.GetLineSpan();
            
            string filePath = !string.IsNullOrEmpty(lineSpan.Path) ? lineSpan.Path : "UnknownScript";

            int line = lineSpan.StartLinePosition.Line + 1;
            int character = lineSpan.StartLinePosition.Character + 1;
            
            string errorCode = d.Id;
            string errorMessage = d.GetMessage();
            
            string formattedError = $"{filePath}({line},{character}): error {errorCode}: {errorMessage}";
            
            Debug.LogError(formattedError);
            Console.WriteLine(formattedError);
        }
        foreach (var d in result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Warning))
        {
            var lineSpan = d.Location.GetLineSpan();
            
            string filePath = !string.IsNullOrEmpty(lineSpan.Path) ? lineSpan.Path : "UnknownScript";

            int line = lineSpan.StartLinePosition.Line + 1;
            int character = lineSpan.StartLinePosition.Character + 1;
            
            string warnCode = d.Id;
            string warnMessage = d.GetMessage();
            
            string formattedWarning = $"{filePath}({line},{character}): warning {warnCode}: {warnMessage}";
            
            Debug.LogWarning(formattedWarning);
            Console.WriteLine(formattedWarning);
        }
        foreach (var d in result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Info))
        {
            var lineSpan = d.Location.GetLineSpan();
            
            string filePath = !string.IsNullOrEmpty(lineSpan.Path) ? lineSpan.Path : "UnknownScript";

            int line = lineSpan.StartLinePosition.Line + 1;
            int character = lineSpan.StartLinePosition.Character + 1;
            
            string infoCode = d.Id;
            string infoMessage = d.GetMessage();
            
            string formattedInfo = $"{filePath}({line},{character}): info {infoCode}: {infoMessage}";
            
            Debug.Log(formattedInfo);
            Console.WriteLine(formattedInfo);
        }
        
        return result.Success ? ms.ToArray() : null;
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
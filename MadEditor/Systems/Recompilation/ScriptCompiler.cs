using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

namespace MadEditor;

public static class ScriptCompiler
{
    public static byte[]? CompileToBytes(string[] sourceFiles)
    {
        var syntaxTrees = sourceFiles
            .Select(f => CSharpSyntaxTree.ParseText(File.ReadAllText(f)))
            .ToArray();

        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a =>
                !a.IsDynamic &&
                !string.IsNullOrEmpty(a.Location) &&
                !a.FullName!.Contains("GameScripts"))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .ToList();

        var compilation = CSharpCompilation.Create(
            "GameScripts",
            syntaxTrees,
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);

        if (!result.Success)
        {
            foreach (var d in result.Diagnostics)
                Console.WriteLine(d);

            return null;
        }

        return ms.ToArray();
    }

    public static Assembly LoadAssembly(byte[] dllBytes, out ScriptLoadContext context)
    {
        context = new ScriptLoadContext();

        using var ms = new MemoryStream(dllBytes);
        return context.LoadFromStream(ms);
    }
}
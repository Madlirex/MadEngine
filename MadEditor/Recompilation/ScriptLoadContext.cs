using System.Reflection;
using System.Runtime.Loader;

namespace MadEditor;

public class ScriptLoadContext : AssemblyLoadContext
{
    public ScriptLoadContext() : base(isCollectible: true) { }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        return Default.Assemblies
            .FirstOrDefault(a => a.GetName().Name == assemblyName.Name);
    }
}
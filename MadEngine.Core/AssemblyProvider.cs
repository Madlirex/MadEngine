using System.Reflection;

namespace MadEngine.Core;

public interface IDomainResetable
{
    void ResetCache();
}

public static class AssemblyProvider
{
    public static IReadOnlyList<Assembly> Assemblies => _assemblies;
    private static List<Assembly> _assemblies = [];

    public static void AddAssembly(Assembly assembly)
    {
        _assemblies.Add(assembly);
    }

    public static void ClearAssemblies()
    {
        _assemblies.Clear();
    }
    
    public static Type? GetType(string typeName)
    {
        foreach (var assembly in Assemblies)
        {
            var type = assembly.GetType(typeName.Split(',')[0].Trim());
            if (type != null) return type;
        }
        return null;
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
using System.Reflection;

namespace MadEditor.PackageManagement;

public static class PackageManager
{
    public static IReadOnlyList<IPackage> Packages => _packages;
    private static List<IPackage> _packages = [];

    public static void RegisterPackages()
    {
        var packages = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(p => typeof(IPackage).IsAssignableFrom(p)
                        && p is { IsClass: true, IsAbstract: false });

        foreach (var package in packages)
        {
            RegisterPackage(package);
        }
    }

    public static void RegisterPackage(IPackage package)
    {
        _packages.Add(package);
    }

    public static void RegisterPackage(Type package)
    {
        IPackage? packageInstance = (IPackage?)Activator.CreateInstance(package);
        
        if(packageInstance == null)
            throw new InvalidOperationException($"Couldn't create an instance of {package.Name}");
        
        RegisterPackage(packageInstance);
    }
}
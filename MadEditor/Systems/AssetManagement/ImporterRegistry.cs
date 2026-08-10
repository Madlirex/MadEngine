namespace MadEditor;

public static class ImporterRegistry
{
    public static IReadOnlyDictionary<Type, IAssetImporter> Importers => _importers;
    private static Dictionary<Type, IAssetImporter> _importers = [];
    
    public static IAssetImporter? GetImporter(Type type)
    {
        Type? currentType = type;
        while (currentType != null)
        {
            if (_importers.TryGetValue(currentType, out IAssetImporter? importer))
                return importer;
            currentType = currentType.BaseType;
        }

        foreach (Type interfaceType in type.GetInterfaces())
        {
            if (_importers.TryGetValue(type, out IAssetImporter? importer))
                return importer;
        }
        return null;
    }

    public static void DiscoverImporters()
    {
        _importers.Clear();
        var importerTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IAssetImporter)
                .IsAssignableFrom(type) && type is { IsAbstract: false, IsInterface: false });

        foreach (Type importerType in importerTypes)
        {
            RegisterImporter(importerType);
        }
    }
    
    public static void RegisterImporter(Type type)
    {
        if (Activator.CreateInstance(type) is IAssetImporter importer)
        {
            _importers.TryAdd(type, importer);
        }
    }
}
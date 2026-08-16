namespace MadEditor;

public static class ImporterRegistry
{
    private static ImporterEngine Instance => RegistryBootstrapper.Get<ImporterEngine>();
    
    public static IAssetImporter? GetImporter(Type type) => Instance.GetImporter(type);
    public static IAssetImporter? GetImporter(string name) => Instance.GetImporter(name);
    public static IAssetImporter? GetImporterByExtension(string extension) => Instance.GetImporterByExtension(extension);
}

internal class ImporterEngine : Registry
{
    public override void Initialize()
    {
        DiscoverImporters();
    }
    
    private readonly Dictionary<Type, IAssetImporter> _importers = [];
    private readonly Dictionary<string, IAssetImporter> _importerNames = [];
    private readonly Dictionary<string, IAssetImporter> _importerExtensions = [];
    
    internal IAssetImporter? GetImporter(Type type)
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
            if (_importers.TryGetValue(interfaceType, out IAssetImporter? importer))
                return importer;
        }
        Console.WriteLine($"Could not find importer for {type}");
        return null;
    }

    internal IAssetImporter? GetImporter(string name)
    {
        Console.WriteLine($"Getting importer for {name}");
        if(!_importerNames.ContainsKey(name)) Console.WriteLine($"Couldn't find importer with name {name}");
        return _importerNames.GetValueOrDefault(name);
    }

    internal IAssetImporter? GetImporterByExtension(string extension)
    {
        if(!_importerExtensions.ContainsKey(extension)) Console.WriteLine($"Couldn't find importer for extension {extension}");
        return _importerExtensions.GetValueOrDefault(extension);
    }

    private void DiscoverImporters()
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
    
    private void RegisterImporter(Type type)
    {
        if (Activator.CreateInstance(type) is IAssetImporter importer)
        {
            _importers.TryAdd(importer.Type, importer);
            _importerNames.TryAdd(importer.Name, importer);
            _importerExtensions.TryAdd(importer.Extension, importer);
        }
    }
}
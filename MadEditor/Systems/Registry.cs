namespace MadEditor;

public abstract class Registry
{
    public abstract void Initialize();

    protected IEnumerable<Type> FindTypesImplementing<TInterface>()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(TInterface).IsAssignableFrom(p) && p is { IsInterface: false, IsAbstract: false });
    }

    protected TInterface InitializeType<TInterface>(Type type, IDictionary<Type, TInterface> instances)
    {
        if (Activator.CreateInstance(type) is not TInterface instance) throw new Exception($"Cannot create instance of type {type}");
        instances.TryAdd(type, instance);
        return instance;
    }
}

public static class RegistryBootstrapper
{
    private static readonly Dictionary<Type, Registry> Instances = [];
    private static bool _initialized;
    
    public static void InitializeAll()
    {
        Instances.Clear();
        
        var registryTypes = ScriptDomain.GetTypesImplementing(typeof(Registry));

        foreach (var registryType in registryTypes)
        {
            Initialize(registryType);
        }
        _initialized = true;
    }

    public static void ReinitializeAll()
    {
        if(!_initialized) return;
        InitializeAll();
    }

    internal static T Get<T>() where T : Registry
    {
        if (Instances.TryGetValue(typeof(T), out var value)) return (T)value;
        return Initialize<T>();
    }

    private static T Initialize<T>() where T : Registry
    {
        return (T)Initialize(typeof(T));
    }

    private static Registry Initialize(Type type)
    {
        if (Activator.CreateInstance(type) is not Registry registry)
            throw new Exception($"Cannot create instance of type {type}");

        Instances.Add(type, registry);
        registry.Initialize();

        return registry;
    }
}
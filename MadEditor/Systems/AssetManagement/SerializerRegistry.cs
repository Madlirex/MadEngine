namespace MadEditor;

public static class SerializerRegistry
{
    public static IReadOnlyDictionary<Type, ISerializer> Serializers => _serializers;
    private static Dictionary<Type, ISerializer> _serializers = [];
    
    public static ISerializer? GetSerializer(Type type)
    {
        Type? currentType = type;
        while (currentType != null)
        {
            if (_serializers.TryGetValue(currentType, out ISerializer? serializer))
                return serializer;
            currentType = currentType.BaseType;
        }

        foreach (Type interfaceType in type.GetInterfaces())
        {
            if (_serializers.TryGetValue(type, out ISerializer? serializer))
                return serializer;
        }
        return null;
    }

    public static void DiscoverSerializers()
    {
        _serializers.Clear();
        var serializerTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(ISerializer)
                .IsAssignableFrom(type) && type is { IsAbstract: false, IsInterface: false });

        foreach (Type serializerType in serializerTypes)
        {
            RegisterSerializer(serializerType);
        }
    }
    
    public static void RegisterSerializer(Type type)
    {
        if (Activator.CreateInstance(type) is ISerializer serializer)
        {
            _serializers.TryAdd(type, serializer);
        }
    }
}
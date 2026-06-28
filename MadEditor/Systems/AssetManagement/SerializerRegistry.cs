namespace MadEditor;

public static class SerializerRegistry
{
    public static IReadOnlyDictionary<Type, ISerializer> Serializers => _serializers;

    private static Dictionary<Type, ISerializer> _serializers = new();

    public static void DiscoverSerializers()
    {
        _serializers.Clear();

        var serializerTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(ISerializer).IsAssignableFrom(p) && p is { IsInterface: false, IsAbstract: false });

        foreach (Type type in serializerTypes)
        {
            var instance = (ISerializer)Activator.CreateInstance(type)!;
            _serializers.Add(instance.ObjectType, instance);
        }
        foreach (var kv in _serializers)
        {
            Console.WriteLine($"{kv.Key} -> {kv.Value.GetType().Name}");
        }
    }

    public static ISerializer GetSerializer(Type type)
    {
        if (_serializers.TryGetValue(type, out var serializer))
            return serializer;

        foreach (var pair in _serializers)
        {
            if (pair.Key.IsAssignableFrom(type))
                return pair.Value;
        }

        throw new Exception($"No serializer for {type}");
    }

    public static ISerializer GetSerializer<T>()
    {
        return GetSerializer(typeof(T));
    }

    public static bool HasSerializer(Type type)
    {
        if (_serializers.ContainsKey(type))
            return true;

        foreach (var key in _serializers.Keys)
        {
            if (key.IsAssignableFrom(type))
                return true;
        }

        return false;
    }

    public static bool HasSerializer<T>()
    {
        return HasSerializer(typeof(T));
    }
}
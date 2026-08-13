using System.Text.Json.Nodes;

namespace MadEditor;

public static class SerializerRegistry
{
    private static readonly SerializerEngine Instance = new();

    public static JsonNode? Serialize(object? obj) => Instance.Serialize(obj);
    public static object? Deserialize(Type type, JsonNode? obj) => Instance.Deserialize(type, obj);
    public static ISerializer? GetSerializer(Type type) => Instance.GetSerializer(type);
    public static T? GetSerializer<T>() where T : ISerializer => Instance.GetSerializer<T>();
    public static IClassSerializer? GetClassSerializer(Type type) => Instance.GetClassSerializer(type);
}

internal class SerializerEngine : Registry
{
    private readonly Dictionary<Type, ISerializer> _serializers = [];
    
    public override void Initialize()
    {
        DiscoverSerializers();
    }
    
    private void DiscoverSerializers()
    {
        _serializers.Clear();
        var serializerTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(ISerializer)
                .IsAssignableFrom(type) && type is { IsAbstract: false, IsInterface: false, IsGenericTypeDefinition: false });

        foreach (Type serializerType in serializerTypes)
        {
            CreateSerializer(serializerType);
        }
    }

    internal ISerializer? CreateSerializer(Type type)
    {
        if (Activator.CreateInstance(type) is not ISerializer serializer) return null;
        _serializers.TryAdd(serializer.Type, serializer);
        return serializer;
    }
    
    internal JsonNode? Serialize(object? obj)
    {
        if (obj == null)
            return null;
        
        ISerializer? serializer = GetSerializer(obj.GetType());

        return serializer switch
        {
            null => JsonValue.Create<object>(null)!,
            IClassSerializer classSerializer => classSerializer.SerializeReference(obj),
            _ => serializer.Serialize(obj)
        };
    }

    internal object? Deserialize(Type targetType, JsonNode? obj)
    {
        if (obj == null || obj is JsonValue value && !value.TryGetValue<object>(out _)) 
            return null;

        ISerializer? serializer = GetSerializer(targetType);

        return serializer switch
        {
            null => null,
            IClassSerializer classSerializer => classSerializer.DeserializeReference(obj),
            _ => serializer.Deserialize(obj)
        };
    }
    
    internal ISerializer? GetSerializer(Type type)
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
            if (_serializers.TryGetValue(interfaceType, out ISerializer? serializer))
                return serializer;
        }
        
        return TryGenerateDynamicSerializer(type);
    }

    internal T? GetSerializer<T>() where T : ISerializer
    {
        return (T?)GetSerializer(typeof(T));
    }

    internal IClassSerializer? GetClassSerializer(Type type)
    {
        ISerializer? serializer = GetSerializer(type);
        
        if (serializer is IClassSerializer classSerializer)
            return classSerializer;
        
        return null;
    }

    private ISerializer? TryGenerateDynamicSerializer(Type type)
    {
        if (type.IsArray)
        {
            Type elementType = type.GetElementType()!;
            Type arraySerializerType = typeof(ArraySerializer<>).MakeGenericType(elementType);
            return CreateSerializer(arraySerializerType);
        }

        if (!type.IsGenericType) return null;
        Type genericDefinition = type.GetGenericTypeDefinition();

        if (genericDefinition == typeof(List<>))
        {
            Type itemType = type.GetGenericArguments()[0];
            Type listSerializerType = typeof(ListSerializer<>).MakeGenericType(itemType);
            return CreateSerializer(listSerializerType);
        }

        if (genericDefinition != typeof(Dictionary<,>)) return null;
            
        Type[] genericArgs = type.GetGenericArguments();
        if (genericArgs[0] != typeof(string)) return null;
                
        Type valueType = genericArgs[1];
        Type dictSerializerType = typeof(DictionarySerializer<>).MakeGenericType(valueType);
        return CreateSerializer(dictSerializerType);
    }
}
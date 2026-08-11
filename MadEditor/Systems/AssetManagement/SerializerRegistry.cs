using System.Text.Json.Nodes;

namespace MadEditor;

public static class SerializerRegistry
{
    public static IReadOnlyDictionary<Type, ISerializer> Serializers => _serializers;
    private static Dictionary<Type, ISerializer> _serializers = [];

    public static JsonNode? Serialize(object? obj)
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

    public static object? Deserialize(Type targetType, JsonNode? obj)
    {
        if (obj == null || obj is JsonValue value && !value.TryGetValue<object>(out _)) 
            return null;

        ISerializer? serializer = GetSerializer(targetType);
        
        if (serializer == null) return null;
        
        if (serializer is IClassSerializer classSerializer)
            return classSerializer.DeserializeReference(obj);
        
        return serializer.Deserialize(obj);
    }
    
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
            if (_serializers.TryGetValue(interfaceType, out ISerializer? serializer))
                return serializer;
        }
        
        return TryGenerateDynamicSerializer(type);
    }

    public static T? GetSerializer<T>() where T : ISerializer
    {
        return (T?)GetSerializer(typeof(T));
    }

    public static IClassSerializer? GetClassSerializer(Type type)
    {
        ISerializer? serializer = GetSerializer(type);
        
        if (serializer is IClassSerializer classSerializer)
            return classSerializer;
        
        return null;
    }

    public static ISerializer? TryGenerateDynamicSerializer(Type type)
    {
        if (type.IsArray)
        {
            Type elementType = type.GetElementType()!;
            Type arraySerializerType = typeof(ArraySerializer<>).MakeGenericType(elementType);
            return CreateSerializer(arraySerializerType);
        }
        
        if (type.IsGenericType)
        {
            Type genericDefinition = type.GetGenericTypeDefinition();

            if (genericDefinition == typeof(List<>))
            {
                Type itemType = type.GetGenericArguments()[0];
                Type listSerializerType = typeof(ListSerializer<>).MakeGenericType(itemType);
                return CreateSerializer(listSerializerType);
            }
            
            if (genericDefinition == typeof(Dictionary<,>))
            {
                Type[] genericArgs = type.GetGenericArguments();
                if (genericArgs[0] == typeof(string))
                {
                    Type valueType = genericArgs[1];
                    Type dictSerializerType = typeof(DictionarySerializer<>).MakeGenericType(valueType);
                    return CreateSerializer(dictSerializerType);
                }
            }
        }
        return null;
    }
    
    public static void DiscoverSerializers()
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

    public static ISerializer? CreateSerializer(Type type)
    {
        if (Activator.CreateInstance(type) is ISerializer serializer)
        {
            _serializers.TryAdd(serializer.Type, serializer);
            return serializer;
        }

        return null;
    }
}
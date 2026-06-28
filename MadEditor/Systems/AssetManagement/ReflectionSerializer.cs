using System.Collections;
using System.Reflection;
using System.Text.Json.Nodes;
using MadEngine.Core;

namespace MadEditor;

public static class ReflectionSerializer
{
    private static HashSet<Guid> _visited = [];
    
    public static JsonObject Serialize(object obj)
    {
        _visited.Clear();
        return SerializeObject(obj);
    }

    public static void DeserializeInto(object obj, JsonObject json, Dictionary<Guid, MadObject> objectMap)
    {
        DeserializeObject(obj, json, objectMap);
    }

    private static JsonObject SerializeObject(object obj)
    {
        if (obj is MadObject mo)
        {
            Console.WriteLine(mo.Guid);
            if (!_visited.Add(mo.Guid))
            {
                return new JsonObject
                {
                    ["Ref"] = mo.Guid.ToString()
                };
            }
        }
        
        JsonObject json = new();
        Type type = obj.GetType();

        foreach (var field in GetFields(type))
        {
            object? value = field.GetValue(obj);
            json[field.Name] = SerializeValue(value);
        }

        foreach (var prop in GetProperties(type))
        {
            if (prop.IsSpecialName)
                continue;
            
            if (!prop.CanRead)
                continue;
            
            if (prop.GetIndexParameters().Length > 0)
                continue;

            try
            {
                object? value = prop.GetValue(obj);
                json[prop.Name] = SerializeValue(value);
            }
            catch
            {
                // ignored
            }
        }

        return json;
    }

    private static JsonNode? SerializeValue(object? value)
    {
        
        if (value == null)
            return null;

        Type type = value.GetType();
        Console.WriteLine(type);

        if (value is MadObject obj)
            return obj.Guid.ToString();
        
        if (SerializerRegistry.HasSerializer(type))
        {
            return SerializerRegistry.GetSerializer(type).Serialize(value);
        }
        
        if (type.IsPrimitive || value is string || value is decimal)
            return JsonValue.Create(value);

        if (value is Guid guid)
            return guid.ToString();

        if (type.IsEnum)
            return value.ToString();
        

        if (value is IEnumerable enumerable and not string)
        {
            JsonArray arr = [];

            foreach (var item in enumerable)
            {
                arr.Add(SerializeValue(item));
            }

            return arr;
        }
        
        if (type.Namespace != null && type.Namespace!.StartsWith("System"))
        {
            if (value is not string && type is { IsPrimitive: false, IsEnum: false })
                return null;
        }
        
        return SerializeObject(value);
    }
    
    private static void DeserializeObject(object obj, JsonObject json, Dictionary<Guid, MadObject> objectMap)
    {
        Type type = obj.GetType();

        foreach (var field in GetFields(type))
        {
            if (!json.TryGetPropertyValue(field.Name, out JsonNode? node))
                continue;

            object? value = DeserializeValue(field.FieldType, node, objectMap);
            field.SetValue(obj, value);
        }

        foreach (var prop in GetProperties(type))
        {
            if (!prop.CanWrite)
                continue;

            if (!json.TryGetPropertyValue(prop.Name, out JsonNode? node))
                continue;

            object? value = DeserializeValue(prop.PropertyType, node, objectMap);
            prop.SetValue(obj, value);
        }
    }

    private static object? DeserializeValue(Type type, JsonNode? node, Dictionary<Guid, MadObject> objectMap)
    {
        if (node == null)
            return null;

        if (typeof(MadObject).IsAssignableFrom(type))
        {
            Guid id = Guid.Parse(node.GetValue<string>());
            return objectMap[id];
        }
        
        if (type == typeof(string))
            return node.GetValue<string>();

        if (type == typeof(int))
            return node.GetValue<int>();

        if (type == typeof(float))
            return node.GetValue<float>();

        if (type == typeof(bool))
            return node.GetValue<bool>();

        if (type == typeof(double))
            return node.GetValue<double>();

        if (type == typeof(Guid))
            return Guid.Parse(node.GetValue<string>());

        if (type.IsEnum)
            return Enum.Parse(type, node.GetValue<string>());

        if (type.IsArray)
        {
            Type elementType = type.GetElementType()!;
            JsonArray arr = (JsonArray)node;

            Array result = Array.CreateInstance(elementType, arr.Count);

            for (int i = 0; i < arr.Count; i++)
            {
                result.SetValue(DeserializeValue(elementType, arr[i], objectMap), i);
            }

            return result;
        }

        if (type.IsGenericType && typeof(IList).IsAssignableFrom(type))
        {
            Type elementType = type.GetGenericArguments()[0];
            IList list = (IList)Activator.CreateInstance(type)!;

            JsonArray arr = (JsonArray)node;

            foreach (var item in arr)
            {
                list.Add(DeserializeValue(elementType, item, objectMap));
            }

            return list;
        }

        object obj = Activator.CreateInstance(type)!;
        DeserializeObject(obj, (JsonObject)node, objectMap);

        return obj;
    }
    
    private static IEnumerable<FieldInfo> GetFields(Type type)
    {
        return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    }

    private static IEnumerable<PropertyInfo> GetProperties(Type type)
    {
        return type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
    }
}
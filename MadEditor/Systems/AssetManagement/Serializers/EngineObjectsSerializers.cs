using System.Reflection;
using System.Text.Json.Nodes;
using MadEngine.Core;

namespace MadEditor;

public class MadObjectSerializer : ClassSerializer<MadObject>
{
    private const BindingFlags EngineFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
    
    public override JsonNode Serialize(MadObject obj)
    {
        JsonObject objJson = new JsonObject
        {
            ["$type"] = obj.GetType().AssemblyQualifiedName,
            ["$guid"] = obj.Guid.ToString()
        };
        
        JsonObject dataJson = new JsonObject();
        
        Type type = obj.GetType();
        
        foreach (var prop in type.GetProperties(EngineFlags))
        {
            if (prop is { CanRead: true, CanWrite: true } && !Attribute.IsDefined(prop, typeof(DoNotSaveAttribute)))
            {
                object? value = prop.GetValue(obj);
                dataJson[prop.Name] = SerializerRegistry.Serialize(value);
            }
        }
        
        foreach (var field in type.GetFields(EngineFlags))
        {
            if (field.IsDefined(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false))
                continue;

            if (Attribute.IsDefined(field, typeof(DoNotSaveAttribute))) continue;
            object? value = field.GetValue(obj);
            dataJson[field.Name] = SerializerRegistry.Serialize(value);
        }

        objJson["$data"] = dataJson;
        return objJson;
    }

    public override JsonNode SerializeReference(MadObject obj)
    {
        return JsonValue.Create(obj.Guid);
    }
    
    public override MadObject Deserialize(JsonNode obj)
    {
        if (obj is not JsonObject jsonObject)
            throw new ArgumentException("Expected a JsonObject node configuration.");
        
        
        string? typeString = jsonObject["$type"]?.GetValue<string>();
        if (string.IsNullOrEmpty(typeString))
            throw new InvalidOperationException("JSON missing required '$type' metadata tag.");
        
        Type? type = Type.GetType(typeString);
        if (type == null)
            throw new TypeLoadException($"Unable to find type '{typeString}'.");

        MadObject instance = (MadObject)Activator.CreateInstance(type)!;
        instance.Guid = jsonObject["$guid"]!.GetValue<Guid>();
        return instance;
    }

    public override void DeserializeInto(MadObject? target, JsonNode source)
    {
        if (source is not JsonObject sourceObject || target == null) return;

        Type type = target.GetType();
        
        foreach (var pair in sourceObject)
        {
            string memberName = pair.Key;
            JsonNode? valueNode = pair.Value;
            
            var prop = type.GetProperty(memberName, EngineFlags);
            if (prop != null && prop.CanWrite && !Attribute.IsDefined(prop, typeof(DoNotSaveAttribute)))
            {
                prop.SetValue(target, SerializerRegistry.Deserialize(prop.PropertyType, valueNode));
            }
            
            var field = type.GetField(memberName, EngineFlags);
            if (field != null && !Attribute.IsDefined(field, typeof(DoNotSaveAttribute)))
            {
                field.SetValue(target, SerializerRegistry.Deserialize(field.FieldType, valueNode));
            }
        }
    }

    public override MadObject DeserializeReference(JsonNode obj)
    {
        return AssetRegistry.GetObject(Guid.Parse(obj.GetValue<string>()))!;
    }
}
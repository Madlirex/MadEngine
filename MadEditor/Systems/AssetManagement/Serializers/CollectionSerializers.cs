using System.Text.Json.Nodes;

namespace MadEditor;

public class ListSerializer<T> : Serializer<List<T>>
{

    public override JsonNode Serialize(List<T> obj)
    {
        var jsonArray = new JsonArray();

        foreach (var item in obj)
        {
            if (item == null)
            {
                jsonArray.Add(JsonValue.Create<object>(null));
            }
            else
            {
                jsonArray.Add(SerializerRegistry.Serialize(item));
            }
        }
        return jsonArray;
    }

    public override List<T> Deserialize(JsonNode obj)
    {
        var list = new List<T>();
        if (obj is not JsonArray jsonArray) return list;

        foreach (var node in jsonArray)
        {
            if (node == null)
            {
                list.Add(default!); 
            }
            else
            {
                list.Add((T)SerializerRegistry.Deserialize(typeof(T), node)!);
            }
        }
        return list;
    }
}

public class ArraySerializer<T> : Serializer<T[]>
{
    public override JsonNode Serialize(T[] obj)
    {
        var jsonArray = new JsonArray();

        foreach (var item in obj)
        {
            if (item == null)
            {
                jsonArray.Add(JsonValue.Create((object?)null));
            }
            else
            {
                jsonArray.Add(SerializerRegistry.Serialize(item));
            }
        }
        return jsonArray;
    }

    public override T[] Deserialize(JsonNode obj)
    {
        if (obj is not JsonArray jsonArray) return Array.Empty<T>();

        var array = new T[jsonArray.Count];
        for (int i = 0; i < jsonArray.Count; i++)
        {
            var node = jsonArray[i];
            if (node == null)
            {
                array[i] = default!;
            }
            else
            {
                array[i] = (T)SerializerRegistry.Deserialize(typeof(T), node)!;
            }
        }
        return array;
    }
}

public class DictionarySerializer<TValue> : Serializer<Dictionary<string, TValue>>
{

    public override JsonNode Serialize(Dictionary<string, TValue> obj)
    {
        var jsonObject = new JsonObject();

        foreach (var pair in obj)
        {
            if (pair.Value == null)
            {
                jsonObject[pair.Key] = JsonValue.Create<object>(null);
            }
            else
            {
                jsonObject[pair.Key] = SerializerRegistry.Serialize(pair.Value);
            }
        }
        return jsonObject;
    }

    public override Dictionary<string, TValue> Deserialize(JsonNode obj)
    {
        var dictionary = new Dictionary<string, TValue>();
        if (obj is not JsonObject jsonObject) return dictionary;

        foreach (var pair in jsonObject)
        {
            if (pair.Value == null)
            {
                dictionary[pair.Key] = default!;
            }
            else
            {
                dictionary[pair.Key] = (TValue)SerializerRegistry.Deserialize(typeof(TValue), pair.Value)!;
            }
        }
        return dictionary;
    }
}
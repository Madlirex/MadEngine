using System.Text.Json.Nodes;

namespace MadEditor;

public class ListSerializer<T> : Serializer<List<T>>
{
    private readonly ISerializer? _itemSerializer = SerializerRegistry.GetSerializer(typeof(T));

    public override JsonNode Serialize(List<T> obj)
    {
        var jsonArray = new JsonArray();

        if (_itemSerializer == null)
            throw new InvalidOperationException($"No serializer found for list item type {typeof(T).Name}");

        foreach (var item in obj)
        {
            if (item == null)
            {
                jsonArray.Add(JsonValue.Create<object>(null));
            }
            else
            {
                jsonArray.Add(_itemSerializer.Serialize(item));
            }
        }
        return jsonArray;
    }

    public override List<T> Deserialize(JsonNode obj)
    {
        var list = new List<T>();
        if (obj is not JsonArray jsonArray) return list;

        if (_itemSerializer == null)
            throw new InvalidOperationException($"No serializer found for list item type {typeof(T).Name}");

        foreach (var node in jsonArray)
        {
            if (node == null)
            {
                list.Add(default!); 
            }
            else
            {
                list.Add((T)_itemSerializer.Deserialize(node)!);
            }
        }
        return list;
    }
}

public class ArraySerializer<T> : Serializer<T[]>
{
    private readonly ISerializer? _itemSerializer = SerializerRegistry.GetSerializer(typeof(T));

    public override JsonNode Serialize(T[] obj)
    {
        var jsonArray = new JsonArray();

        if (_itemSerializer == null)
            throw new InvalidOperationException($"No serializer found for array element type {typeof(T).Name}");

        foreach (var item in obj)
        {
            if (item == null)
            {
                jsonArray.Add(JsonValue.Create((object?)null));
            }
            else
            {
                jsonArray.Add(_itemSerializer.Serialize(item));
            }
        }
        return jsonArray;
    }

    public override T[] Deserialize(JsonNode obj)
    {
        if (obj is not JsonArray jsonArray) return Array.Empty<T>();

        if (_itemSerializer == null)
            throw new InvalidOperationException($"No serializer found for array element type {typeof(T).Name}");

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
                array[i] = (T)_itemSerializer.Deserialize(node)!;
            }
        }
        return array;
    }
}

public class DictionarySerializer<TValue> : Serializer<Dictionary<string, TValue>>
{
    private readonly ISerializer? _valueSerializer = SerializerRegistry.GetSerializer(typeof(TValue));

    public override JsonNode Serialize(Dictionary<string, TValue> obj)
    {
        var jsonObject = new JsonObject();

        if (_valueSerializer == null)
            throw new InvalidOperationException($"No serializer found for dictionary value type {typeof(TValue).Name}");

        foreach (var pair in obj)
        {
            if (pair.Value == null)
            {
                jsonObject[pair.Key] = JsonValue.Create<object>(null);
            }
            else
            {
                jsonObject[pair.Key] = _valueSerializer.Serialize(pair.Value);
            }
        }
        return jsonObject;
    }

    public override Dictionary<string, TValue> Deserialize(JsonNode obj)
    {
        var dictionary = new Dictionary<string, TValue>();
        if (obj is not JsonObject jsonObject) return dictionary;

        if (_valueSerializer == null)
            throw new InvalidOperationException($"No serializer found for dictionary value type {typeof(TValue).Name}");

        foreach (var pair in jsonObject)
        {
            if (pair.Value == null)
            {
                dictionary[pair.Key] = default!;
            }
            else
            {
                dictionary[pair.Key] = (TValue)_valueSerializer.Deserialize(pair.Value)!;
            }
        }
        return dictionary;
    }
}
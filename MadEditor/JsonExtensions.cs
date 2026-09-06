using System.Text.Json.Nodes;

namespace MadEditor;

public static class JsonNodeExtensions
{
    public static Guid GetGuid(this JsonNode node)
    {
        if (node is JsonValue jsonValue && jsonValue.TryGetValue(out string? guidStr))
        {
            return Guid.Parse(guidStr);
        }
        
        return node.GetValue<Guid>();
    }
}

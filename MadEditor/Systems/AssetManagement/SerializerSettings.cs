using System.Text.Json;

namespace MadEditor;

public static class SerializerSettings
{
    public static JsonSerializerOptions SerializerOptions => new JsonSerializerOptions()
    {
        WriteIndented = true,
        IndentSize = 4,
        IncludeFields = true
    };
}
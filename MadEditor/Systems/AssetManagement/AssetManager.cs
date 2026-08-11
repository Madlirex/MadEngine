using MadEngine.Core;

namespace MadEditor;

public static class AssetManager
{
    public static IReadOnlyDictionary<Guid, MadObject> Objects => _objects;
    private static Dictionary<Guid, MadObject> _objects = new();

    public static MadObject? GetObject(Guid guid)
    {
        return _objects.GetValueOrDefault(guid);
    }
    
    public static void AddObject(MadObject obj)
    {
        if (!_objects.ContainsKey(obj.Guid))
            _objects.Add(obj.Guid, obj);
        else
            Console.WriteLine("WARNING: Duplicate object: " + obj.Guid);
    }

    public static void RemoveObject(Guid guid)
    {
        if (!_objects.ContainsKey(guid))
        {
            Console.WriteLine("ERROR: Object not found: " + guid);
            return;
        }
        _objects.Remove(guid);
    }

    public static void RemoveObject(MadObject obj)
    {
        RemoveObject(obj.Guid);
    }
}
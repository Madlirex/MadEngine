namespace MadEngine.Core;

public static class AssetRegistry
{
    public static Asset[] Assets => _assets.Values.ToArray();
    
    private static Dictionary<string, Guid> _guidByPath = new();
    private static Dictionary<Guid, string> _pathByGuid = new();
    private static Dictionary<Guid, Asset> _assets = new();
    private static Dictionary<Type, List<Asset>> _assetRegistries = new();

    public static Dictionary<Guid, MadObject> ObjectMap => _objectMap;
    private static Dictionary<Guid, MadObject> _objectMap = new();
    
    public static void RegisterAsset(Asset asset)
    {
        GetUniquePath(asset);
        
        _guidByPath.Add(asset.Path, asset.Guid);
        _pathByGuid.Add(asset.Guid, asset.Path);
        _assets.Add(asset.Guid, asset);

        if (!_assetRegistries.TryGetValue(asset.GetType(), out List<Asset>? value))
        {
            value = [];
            _assetRegistries[asset.GetType()] = value;
        }
    
        value.Add(asset);
    }

    public static void UnregisterAsset(Asset asset)
    {
        string path = asset.Path;
        _guidByPath.Remove(path);
        _pathByGuid.Remove(asset.Guid);
        _assets.Remove(asset.Guid);
        
        _assetRegistries[asset.GetType()].Remove(asset);
    }

    public static void GetUniquePath(Asset asset)
    {
        Console.WriteLine($"Finding for: {asset.Path}");
        if (!_guidByPath.ContainsKey(asset.Path))
            return;

        int i = 1;
        string newPath = Path.Combine(asset.Directory, $"{asset.Name}_{i}{asset.Extension}");
        
        while (_guidByPath.ContainsKey(newPath))
        {
            i++;
            newPath = Path.Combine(asset.Directory, $"{asset.Name}_{i}{asset.Extension}");
        }

        asset.Name = asset.Name + "_" + i;
    }

    public static void RegisterObject(MadObject obj)
    {
        _objectMap.Add(obj.Guid, obj);
    }

    public static void UnregisterObject(MadObject obj)
    {
        _objectMap.Remove(obj.Guid);
    }
    
    public static Asset GetAsset(Guid guid)
    {
        return _assets[guid];
    }
    
    public static Guid GetGuid(string path)
    {
        return _guidByPath[path];
    }

    public static string GetPath(Guid guid)
    {
        return _pathByGuid[guid];
    }
}
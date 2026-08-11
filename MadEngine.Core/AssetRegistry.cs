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
        
        if (_assets.ContainsKey(asset.Guid))
        {
            Console.WriteLine($"[Warning] Duplicate GUID {asset.Guid} found for asset '{asset.AbsolutePath}'. Regenerating a fresh runtime GUID.");
            asset.Guid = Guid.NewGuid(); 
        }
        
        if (_guidByPath.ContainsKey(asset.AbsolutePath))
        {
            Console.WriteLine($"[Error] Path collision skipped: {asset.AbsolutePath}");
            return;
        }
    
        _guidByPath.Add(asset.AbsolutePath, asset.Guid);
        _pathByGuid.Add(asset.Guid, asset.AbsolutePath);
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
        string path = asset.AbsolutePath;
        _guidByPath.Remove(path);
        _pathByGuid.Remove(asset.Guid);
        _assets.Remove(asset.Guid);
        
        _assetRegistries[asset.GetType()].Remove(asset);
    }

    public static void GetUniquePath(Asset asset)
    {
        if (!_guidByPath.ContainsKey(asset.AbsolutePath))
            return;

        int i = 1;
        string newPath = Path.Combine(asset.FullDir, $"{asset.Name}_{i}{asset.Extension}");
        
        while (_guidByPath.ContainsKey(newPath))
        {
            i++;
            newPath = Path.Combine(asset.FullDir, $"{asset.Name}_{i}{asset.Extension}");
        }

        asset.Name = asset.Name + "_" + i;
    }

    public static void RegisterObject(MadObject obj)
    {
        if (_assets.ContainsKey(obj.Guid))
        {
            Console.WriteLine($"[Warning] Duplicate GUID {obj.Guid} found for object {obj.Name}'. Regenerating a fresh runtime GUID.");
            obj.Guid = Guid.NewGuid(); 
        }   
        _objectMap.Add(obj.Guid, obj);
    }

    public static void UnregisterObject(MadObject obj)
    {
        _objectMap.Remove(obj.Guid);
    }

    public static MadObject? GetObject(Guid guid)
    {
        return _objectMap.GetValueOrDefault(guid);
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
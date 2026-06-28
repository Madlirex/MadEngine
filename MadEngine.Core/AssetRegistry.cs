namespace MadEngine.Core;

public static class AssetRegistry
{
    public static Asset[] Assets => _assets.Values.ToArray();
    
    private static Dictionary<string, Guid> _guidByPath = new();
    private static Dictionary<Guid, string> _pathByGuid = new();
    private static Dictionary<Guid, Asset> _assets = new();
    private static Dictionary<Type, List<Asset>> _assetRegistries = new();

    public static Dictionary<Guid, MadObject> ObjectMap = new();
    
    public static void RegisterAsset(Asset asset, string path)
    {
        _guidByPath.Add(path, asset.Guid);
        _pathByGuid.Add(asset.Guid, path);
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
        string path = GetPath(asset.Guid);
        _guidByPath.Remove(path);
        _pathByGuid.Remove(asset.Guid);
        _assets.Remove(asset.Guid);
        
        _assetRegistries[asset.GetType()].Remove(asset);
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
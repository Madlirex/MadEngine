using System.Collections.ObjectModel;

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
    
    public static void RegisterAsset(Asset asset, string path)
    {
        asset.Path = path;
        
        _guidByPath.Add(path, asset.Guid);
        _pathByGuid.Add(asset.Guid, path);
        _assets.Add(asset.Guid, asset);
        
        RegisterObject(asset);
        if (!_assetRegistries.TryGetValue(asset.GetType(), out List<Asset>? value))
        {
            value = [];
            _assetRegistries[asset.GetType()] = value;
        }
    
        value.Add(asset);
    }

    public static void RegisterAsset(Asset asset)
    {
        RegisterAsset(asset, asset.Path);
    }

    public static void UnregisterAsset(Asset asset)
    {
        string path = GetPath(asset.Guid);
        _guidByPath.Remove(path);
        _pathByGuid.Remove(asset.Guid);
        _assets.Remove(asset.Guid);
        
        _assetRegistries[asset.GetType()].Remove(asset);
        UnregisterObject(asset);
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
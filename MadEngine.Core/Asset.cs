namespace MadEngine.Core;

public abstract class Asset : MadObject
{
    public override string Name { get; set; } = "NewAsset";
    public string Path => Directory + @"\" + Name + Extension;
    public virtual string Extension => ".asset";
    public string Directory { get; set; } = "";

    ~Asset()
    {
        AssetRegistry.UnregisterAsset(this);
    }
}
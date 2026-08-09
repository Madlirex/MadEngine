namespace MadEngine.Core;

public abstract class Asset : MadObject
{
    public override string Name { get; set; } = "NewAsset";
    public string AbsolutePath => FullDir + @"\" + Name + Extension;
    public string RelativePath => RelativeDir + @"\" + Name + Extension;
    public virtual string Extension => ".asset";
    public string FullDir { get; set; } = Application.Directory;
    public string RelativeDir => FullDir.Replace(Application.Directory, "");

    public Asset()
    {
        AssetRegistry.RegisterAsset(this);
    }

    ~Asset()
    {
        AssetRegistry.UnregisterAsset(this);
    }
}
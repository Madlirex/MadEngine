namespace MadEngine.Core;

public abstract class Asset : MadObject
{
    public override string Name { get; set; } = "NewAsset";
    [DoNotSave]
    public string AbsolutePath => FullDir + @"\" + Name + Extension;
    [DoNotSave]
    public string RelativePath => RelativeDir + @"\" + Name + Extension;
    [DoNotSave]
    public virtual string Extension => ".asset";
    [DoNotSave]
    public string FullDir { get; set; } = Application.Directory;
    [DoNotSave]
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
namespace MadEngine.Core;

public abstract class Asset : MadObject
{
    public override string Name { get; set; } = "NewAsset";
    [DoNotSave]
    public string AbsolutePath => Path.Combine(FullDir, $"{Name}{Extension}");
    [DoNotSave]
    public string RelativePath => Path.Combine(RelativeDir, $"{Name}{Extension}");
    [DoNotSave]
    public virtual string Extension => ".asset";
    [DoNotSave]
    public string FullDir { get; set; } = Application.AssetsPath;
    [DoNotSave]
    public string RelativeDir => FullDir.Replace(Application.AssetsPath, "");

    public Asset()
    {
        AssetRegistry.RegisterAsset(this);
    }

    ~Asset()
    {
        AssetRegistry.UnregisterAsset(this);
    }
}
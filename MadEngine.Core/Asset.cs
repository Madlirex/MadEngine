namespace MadEngine.Core;

public abstract class Asset : MadObject
{
    protected override string NameInternal { get; set; } = "NewAsset";
    [DoNotSave] public string AbsolutePath => Path.Combine(FullDir, $"{Name}{Extension}");
    [DoNotSave] public string RelativePath => Path.Combine(RelativeDir, $"{Name}{Extension}");
    [DoNotSave] public string Extension
    {
        get => ExtensionInternal;
        set
        {
            AssetRegistry.Unregister(this);
            ExtensionInternal = value;
            AssetRegistry.Register(this);
        }
    }
    [DoNotSave] protected virtual string ExtensionInternal { get; set; }= ".asset";
    [DoNotSave] public string FullDir
    {
        get => _fullDir;
        set
        {
            AssetRegistry.Unregister(this);
            _fullDir = value;
            AssetRegistry.Register(this);
        }
    }
    [DoNotSave] private string _fullDir = Application.AssetsPath;
    [DoNotSave] public string RelativeDir => FullDir.Replace(Application.AssetsPath, "");

    public Asset()
    {
        AssetRegistry.RegisterAsset(this);
    }

    ~Asset()
    {
        Dispose(false);
    }

    protected override void OnDispose(bool disposing)
    {
        if (disposing)
        {
            AssetRegistry.UnregisterAsset(this);
        }
        base.OnDispose(disposing);
    }
}
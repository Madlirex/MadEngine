namespace MadEngine.Core;

public abstract class Asset : MadObject
{
    [DoNotSave] private bool _initialized;
    
    protected override string NameInternal { get; set; } = "NewAsset";
    [DoNotSave] public string AbsolutePath => Path.Combine(FullDir, $"{Name}{Extension}");
    [DoNotSave] public string RelativePath => Path.Combine(RelativeDir, $"{Name}{Extension}");
    [DoNotSave] public string Extension
    {
        get => ExtensionInternal;
        set
        {
            if(_initialized) AssetRegistry.Unregister(this);
            ExtensionInternal = value;
            if(_initialized) AssetRegistry.Register(this);
        }
    }
    [DoNotSave] protected virtual string ExtensionInternal { get; set; }= ".asset";
    [DoNotSave] public string FullDir
    {
        get => _fullDir;
        set
        {
            if(_initialized) AssetRegistry.Unregister(this);
            _fullDir = value;
            if(_initialized) AssetRegistry.Register(this);
        }
    }
    [DoNotSave] private string _fullDir = Application.AssetsPath;
    [DoNotSave] public string RelativeDir => FullDir.Replace(Application.AssetsPath, "");

    public Asset() : base(false) 
    {
        if(SuppressRegistration) return;
        AssetRegistry.Register(this);
        _initialized = true;
    }

    public Asset(bool autoRegister) : base(autoRegister)
    {
        if (autoRegister) _initialized = true;
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
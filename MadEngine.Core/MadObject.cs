namespace MadEngine.Core;

public abstract class MadObject : IDisposable
{
    private bool _initialized;
    
    public string Name
    {
        get => NameInternal;
        set
        {
            if(_initialized) AssetRegistry.Unregister(this);
            NameInternal = value;
            if(_initialized) AssetRegistry.Register(this);
        }
    }

    protected virtual string NameInternal { get; set; } = "NewObject";

    private Guid _guid = Guid.NewGuid();
    public Guid Guid { get => _guid; set => SetGuid(value); }

    [DoNotSave] protected bool Disposed { get; private set; }
    [DoNotSave] public static bool SuppressRegistration { get; set; } = false;

    public MadObject()
    {
        if (SuppressRegistration) return;
        AssetRegistry.Register(this);
        _initialized = true;
    }
    
    public MadObject(bool autoRegister)
    {
        if (!autoRegister) return;
        AssetRegistry.Register(this);
        _initialized = true;
    }

    public void EndInit()
    {
        if (_initialized) return;
        AssetRegistry.Register(this);
        _initialized = true;
    }
    
    ~MadObject()
    {
        Dispose(false);
    }

    public void Destroy()
    {
        Dispose();
    }
    
    public void SetGuid(Guid guid)
    {
        AssetRegistry.Unregister(this);
        _guid = guid;
        AssetRegistry.Register(this); 
    }

    public override string ToString()
    {
        return $"{Name}##{Guid}";
    }

    protected virtual void OnDispose(bool disposing) {}
    
    protected void Dispose(bool disposing)
    {
        if (Disposed) return;

        if (disposing)
        {
            AssetRegistry.Unregister(this);
        }
        
        OnDispose(disposing);
        Disposed = true;
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
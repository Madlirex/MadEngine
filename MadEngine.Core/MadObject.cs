namespace MadEngine.Core;

public abstract class MadObject : IDisposable
{
    public virtual string Name { get; set; } = "NewObject";

    private Guid _guid = Guid.NewGuid();
    public Guid Guid { get => _guid; set => SetGuid(value); }

    [DoNotSave] protected bool Disposed { get; private set; }

    public MadObject()
    {
        AssetRegistry.RegisterObject(this);
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
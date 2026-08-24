namespace MadEngine.Core;

public abstract class MadObject
{
    public virtual string Name { get; set; } = "NewObject";

    private Guid _guid = Guid.NewGuid();
    public Guid Guid { get => _guid; set => SetGuid(value); }

    public MadObject()
    {
        AssetRegistry.RegisterObject(this);
    }

    ~MadObject()
    {
        AssetRegistry.UnregisterObject(this);
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
}
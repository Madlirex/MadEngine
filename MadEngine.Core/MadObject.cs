namespace MadEngine.Core;

public abstract class MadObject
{
    public virtual string Name { get; set; } = "NewObject";

    public Guid Guid = Guid.NewGuid();
}
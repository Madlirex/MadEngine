namespace MadEngine.Core;

public abstract class Object
{
    public string Name { get; set; } = "NewObject";

    public Guid Guid = Guid.NewGuid();
}
namespace MadEditor;

public interface IInspectorDrawer
{
    public Type Type { get; }
    public void Draw(object selected);
}

public abstract class InspectorDrawer<T>
{
    public abstract void Draw(T selected);
}
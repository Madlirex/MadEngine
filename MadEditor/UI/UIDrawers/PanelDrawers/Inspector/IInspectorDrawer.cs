using MadEngine.Core;

namespace MadEditor;

public interface IInspectorDrawer
{
    Type Type { get; }
    void Draw(EditorUIContext context);
}

public abstract class InspectorDrawer<T> : IInspectorDrawer where T : MadObject
{
    public Type Type => typeof(T);
    public abstract void Draw(EditorUIContext context);
}
using MadEngine.Core;

namespace MadEditor;

public interface IInspectorDrawer
{
    Type Type { get; }
    void Draw(object selected);
}

public abstract class InspectorDrawer<T> : PanelDrawer, IInspectorDrawer where T : MadObject
{
    public abstract Type Type { get; }

    public abstract void Draw(T selected);
    void IInspectorDrawer.Draw(object selected) => Draw((T)selected);
}
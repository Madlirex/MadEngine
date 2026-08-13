using MadEngine.Core;

namespace MadEditor;

public abstract class PanelDrawer
{
    public Guid Guid { get; } = Guid.NewGuid();
    public abstract string Name { get; }
    public abstract PanelRegion PanelRegion { get; set; }
    public abstract void Draw(EditorUIContext context);

    public override string ToString()
    {
        return $"{Name}##{Guid}";
    }
}
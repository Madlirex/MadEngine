using System.Numerics;

namespace MadEditor;

public abstract class PanelDrawer
{
    public Guid Guid { get; } = Guid.NewGuid();
    public virtual PanelRegion PanelRegion { get; set; } = PanelRegion.Floating;
    public virtual Vector2 MinSize { get; } = new(500, 400);
    public abstract void Draw(EditorUIContext context);

    public override string ToString()
    {
        return $"{this.GetCustomName()}##{Guid}";
    }
}
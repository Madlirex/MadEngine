using MadEngine.Core;

namespace MadEditor;

public interface IPanelDrawer
{
    public string Name { get; }
    public PanelRegion PanelRegion { get; set; }
    public void Draw(EditorUIContext context);
}
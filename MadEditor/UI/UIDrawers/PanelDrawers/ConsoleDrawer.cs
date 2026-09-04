using MadEngine.Core;

namespace MadEditor;

[CustomName("Console")]
public class ConsoleDrawer : PanelDrawer
{
    public override PanelRegion PanelRegion { get; set; } = PanelRegion.Bottom;
    public override void Draw(EditorUIContext context)
    {
        throw new NotImplementedException();
    }
}
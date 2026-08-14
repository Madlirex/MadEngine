using ImGuiNET;
using MadEditor;

namespace MadEngine.Input.Editor;

public class TestScript : PanelDrawer
{
    public override string Name => "Test";
    public override PanelRegion PanelRegion { get; set; } = PanelRegion.Bottom;
    public override void Draw(EditorUIContext context)
    {
        ImGui.Text(Name);
    }
}
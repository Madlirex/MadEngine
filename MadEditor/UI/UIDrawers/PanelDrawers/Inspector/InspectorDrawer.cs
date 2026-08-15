using System.Reflection;
using ImGuiNET;
using MadEngine.Core;

namespace MadEditor;

[CustomName("Inspector")]
public class InspectorPanelDrawer : PanelDrawer
{
    public override string Name => "Inspector";
    public override PanelRegion PanelRegion { get; set; } = PanelRegion.Right;

    public override void Draw(EditorUIContext context)
    {
        IInspectorDrawer drawer = InspectorDrawersRegistry.GetDrawer(context.Selected?.GetType());
        drawer.Draw(context);
    }
}
using System.Reflection;
using ImGuiNET;
using MadEngine.Core;

namespace MadEditor;

public class InspectorPanelDrawer : PanelDrawer
{
    public override string Name => "Inspector";
    public override PanelRegion PanelRegion { get; set; } = PanelRegion.Right;

    public override void Draw(EditorUIContext context)
    {
        Console.WriteLine(context.Selected?.GetCustomName());
        IInspectorDrawer drawer = InspectorDrawersRegistry.GetDrawer(context.Selected?.GetType());
        drawer.Draw(context);
    }
}
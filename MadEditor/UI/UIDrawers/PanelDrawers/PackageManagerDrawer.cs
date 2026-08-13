using ImGuiNET;
using MadEditor.PackageManagement;

namespace MadEditor;

public class PackageManagerDrawer : PanelDrawer
{
    public override string Name => "Package Manager";
    public override PanelRegion PanelRegion { get; set; }
    public override void Draw(EditorUIContext context)
    {
        foreach (PackageMeta meta in PackageManager.PackageMetas)
        {
            ImGui.Text(meta.Name);
            ImGui.Text(meta.Description);
        }
    }
}
using ImGuiNET;
using MadEditor.PackageManagement;
using MadEngine.Core;

namespace MadEditor;

[CustomName("Package Manager")]
public class PackageManagerDrawer : PanelDrawer
{
    public override string Name => "Package Manager";
    public override PanelRegion PanelRegion { get; set; }
    public override void Draw(EditorUIContext context)
    {
        foreach (PackageMeta meta in PackageManager.PackagesMetas.Values)
        {
            ImGui.Text(meta.Name);
            ImGui.Text(meta.Description);
        }
    }
}
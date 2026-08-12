using ImGuiNET;
using MadEditor.PackageManagement;

namespace MadEditor;

public class PackageManagerDrawer : IPanelDrawer
{
    public string Name => "Package Manager";
    public PanelRegion PanelRegion { get; set; }
    public void Draw(EditorUIContext context)
    {
        foreach (PackageMeta meta in PackageManager.PackageMetas)
        {
            ImGui.Text(meta.Name);
            ImGui.Text(meta.Description);
        }
    }
}
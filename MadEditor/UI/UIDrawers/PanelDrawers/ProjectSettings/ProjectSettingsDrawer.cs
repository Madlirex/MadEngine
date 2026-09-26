using ImGuiNET;
using MadEngine.Core;

namespace MadEditor;

[CustomName("Project Settings")]
public class ProjectSettingsDrawer : PanelDrawer
{
    public override void Draw(EditorUIContext context)
    {
        if (ImGui.CollapsingHeader("Project Settings", ImGuiTreeNodeFlags.DefaultOpen))
        {
            ImGui.Text("Project Name:");
        }
    }
}
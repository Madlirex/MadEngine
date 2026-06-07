using ImGuiNET;

namespace MadEditor;

public class HierarchyPopup : Popup
{
    public override string Name  => "Hierarchy Popup";
    protected override void Body(EditorUIContext context)
    {
        if (ImGui.MenuItem("Add Empty"))
        {
            context.EnqueueCommand(new CreateGameObjectCommand(context.RightClicked?.Transform));
        }

        if (context.RightClicked == null) return;

        if (ImGui.MenuItem("Delete"))
        {
            context.EnqueueCommand(new DeleteGameObjectCommand(context.RightClicked));
        }
    }
}
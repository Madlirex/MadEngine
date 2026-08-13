using ImGuiNET;
using MadEngine.Core;

namespace MadEditor;

public class HierarchyPopup : Popup
{
    public override string Name  => "Hierarchy Popup";
    protected override void Body(EditorUIContext context)
    {
        if(context.RightClicked is not GameObject gameObject) return;
        if (ImGui.MenuItem("Add Empty"))
        {
            context.EnqueueCommand(new CreateGameObjectCommand(gameObject.Transform));
        }

        if (ImGui.MenuItem("Delete"))
        {
            context.EnqueueCommand(new DeleteGameObjectCommand(gameObject));
        }
    }
}
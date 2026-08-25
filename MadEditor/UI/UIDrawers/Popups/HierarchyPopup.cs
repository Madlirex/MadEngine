using ImGuiNET;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;

namespace MadEditor;

public class HierarchyPopup : Popup
{
    public override string Name  => "HierarchyPopup";
    protected override void Body(EditorUIContext context)
    {
        PopupCommandsRegistry.RenderContextMenu(context.RightClicked);
    }
}
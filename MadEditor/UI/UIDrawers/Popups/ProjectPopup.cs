using ImGuiNET;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;

namespace MadEditor;

public class ProjectPopup : Popup
{
    public override string Name  => "ProjectPopup";
    protected override void Body(EditorUIContext context)
    {
        PopupCommandsRegistry.RenderContextMenu(context.RightClicked);
    }
}
using ImGuiNET;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;

namespace MadEditor;

public class HierarchyPopup : Popup
{
    public override string Name  => "Hierarchy Popup";
    protected override void Body(EditorUIContext context)
    {
        if (ImGui.MenuItem("Add Empty"))
        {
            GameObject newObj = new GameObject();
            SceneManager.ActiveScene.Add(newObj);
            if (context.RightClicked != null) newObj.Transform.Parent = context.RightClicked.Transform.Parent;
        }
    }
}
using ImGuiNET;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;

namespace MadEditor;

public class HierarchyDrawer : IPanelDrawer
{
    public string Name => "Hierarchy";
    public PanelRegion PanelRegion { get; set; } = PanelRegion.Left;
    public void Draw(EditorUIContext context)
    {
        var objects = SceneManager.ActiveScene.GameObjects;
        foreach (var go in objects)
        {
            string label = go.Name + "##" + go.Id;
            bool sel = go == context.Selected;
            if (ImGui.Selectable(label, sel))
                context.Selected = go;
        }
    }
}
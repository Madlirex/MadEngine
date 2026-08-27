using System.Reflection;
using ImGuiNET;
using MadEngine.Core;

namespace MadEditor;

public class DefaultInspectorDrawer : InspectorDrawer<MadObject>
{
    public override void Draw(EditorUIContext context)
    {
        if(context.Selected == null) return;
            
        DrawHeader(context.Selected);
        DrawBody(context.Selected);
        DrawFooter(context);
    }

    public void DrawHeader(MadObject selected)
    {
        ImGui.Text("Name: " + selected.Name);
        ImGuiEx.SelectableTextDisabled("ID: " + selected.Guid);
        ImGui.Separator();
    }

    public void DrawBody(MadObject selected)
    {
        FieldDrawingManager.Render(selected);
    }

    public void DrawFooter(EditorUIContext context)
    {
        if (ImGui.Button("Recompile Scripts"))
        {
            AssetManager.RecompileScripts();
        }
    }
}
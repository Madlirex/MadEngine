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
        ImGui.Text("ID: " + selected.Guid);
        ImGui.Separator();
    }

    public void DrawBody(MadObject selected)
    {
        IOrderedEnumerable<InspectorMember> members = selected.GetType()
            .GetFields(BindingFlags.Instance | BindingFlags.Public)
            .Where(f => f.GetCustomAttribute<HideInInspectorAttribute>() == null)
            .Select(f => (InspectorMember)new FieldMember(f))
            .Concat(
                selected.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => p.GetCustomAttribute<ShowInInspectorAttribute>() != null)
                    .Select(p => (InspectorMember)new PropertyMember(p))
            )
            .OrderBy(m => m.Order);
        foreach (InspectorMember member in members)
        {
            if (FieldDrawerRegistry.TryGetDrawer(member.Type, out FieldDrawer drawer))
            {
                FieldDrawerManager.Draw(selected, drawer, member);
            }
        }
    }

    public void DrawFooter(EditorUIContext context)
    {
        if (ImGui.Button("Recompile Scripts"))
        {
            AssetManager.RecompileScripts();
        }
    }
}
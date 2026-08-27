using System.Reflection;
using ImGuiNET;
using MadEngine.Core;

namespace MadEditor;

public class GameObjectDrawer : InspectorDrawer<GameObject>
{
    public readonly AddComponentPopup AddComponentPopup = new();
    
    public override void Draw(EditorUIContext context)
    {
        if(context.Selected == null) return;
            
        DrawHeader(context.Selected);
        DrawBody((GameObject)context.Selected);
        DrawFooter(context);
    }

    public void DrawHeader(MadObject selected)
    {
        string name = selected.Name;
        if (ImGui.InputText("Name", ref name, 128))
            selected.Name = name;
        ImGuiEx.SelectableTextDisabled("ID: " + selected.Guid);
        ImGui.Separator();
    }

    public void DrawBody(GameObject selected)
    {
        DrawComponents(selected);
    }

    public void DrawComponents(GameObject selected)
    {
        foreach (Component component in selected.Components.ToArray())
        {
            DrawComponent(component, selected);
        }
    }

    public void DrawComponent(Component component, GameObject selected)
    {
        ImGui.PushID(component.Guid.ToString());

        string name = component.GetCustomName();

        if (ImGui.BeginTable("ComponentHeader", 2, ImGuiTableFlags.SizingStretchProp))
        {
            ImGui.TableSetupColumn("Name");
            ImGui.TableSetupColumn("Actions", ImGuiTableColumnFlags.WidthFixed, 30);

            ImGui.TableNextRow();

            ImGui.TableSetColumnIndex(0);

            bool open = ImGui.CollapsingHeader(
                name,
                ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.SpanAvailWidth
            );

            ImGui.TableSetColumnIndex(1);

            ImGui.PushStyleColor(ImGuiCol.Button, new System.Numerics.Vector4(0, 0, 0, 0));

            bool removeClicked = ComponentRules.CanBeRemoved(component.GetType()) && ImGui.SmallButton("X");

            ImGui.PopStyleColor();

            ImGui.EndTable();

            if (removeClicked)
            {
                component.GameObject.RemoveComponent(component);
                ImGui.PopID();
                return;
            }

            if (open)
            {
                FieldDrawingManager.Render(component, selected);

                ImGui.Separator();
            }
        }

        ImGui.PopID();
    }

    public void DrawFooter(EditorUIContext context)
    {
        if (ImGui.Button("Add Component"))
        {
            AddComponentPopup.Open();
        }

        if (ImGui.Button("Recompile Scripts"))
        {
            AssetManager.RecompileScripts();
        }

        AddComponentPopup.Draw(context);
    }

}
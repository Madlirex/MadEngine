using System.Reflection;
using ImGuiNET;
using MadEngine.Core;

namespace MadEditor;

public class InspectorDrawer : IPanelDrawer
{
    public string Name => "Inspector";
    public PanelRegion PanelRegion { get; set; } = PanelRegion.Right;
    public AddComponentPopup AddComponentPopup = new();

    public void Draw(EditorUIContext context)
    {
        if (context.Selected == null)
        {
            ImGui.TextDisabled("Select an object in the Hierarchy.");
            return;
        }

        DrawHeader(context.Selected);
        DrawComponents(context.Selected);
        DrawFooter(context);
    }

    public void DrawHeader(GameObject selected)
    {
        string name = selected.Name;
        if (ImGui.InputText("Name", ref name, 128))
            selected.Name = name;
        ImGui.Text("ID: " + selected.Id);
        ImGui.Separator();
    }

    public void DrawComponents(GameObject selected)
    {
        foreach (Component component in selected.Components.ToArray())
        {
            DrawComponent(component);
        }
    }

    public void DrawComponent(Component component)
    {
        ImGui.PushID(component.Id.ToString());

        string name = component.GetType().Name;

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
                foreach (FieldInfo field in component.GetType()
                             .GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (FieldDrawerRegistry.TryGetDrawer(field.FieldType, out FieldDrawer drawer))
                    {
                        drawer.Draw(component, field, component);
                    }
                }

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

        AddComponentPopup.Draw(context);
    }
}
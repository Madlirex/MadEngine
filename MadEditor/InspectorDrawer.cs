using System.Reflection;
using ImGuiNET;
using MadEngine.Core;

namespace MadEditor;

public static class InspectorDrawer
{
    public static void Draw(GameObject? selected)
    {
        if (selected == null)
        {
            ImGui.TextDisabled("Select an object in the Hierarchy.");
            return;
        }

        DrawHeader(selected);
        DrawComponents(selected);
        DrawFooter(selected);
    }

    public static void DrawHeader(GameObject selected)
    {
        string name = selected.Name;
        if (ImGui.InputText("Name", ref name, 128))
            selected.Name = name;
        ImGui.Text("ID: " + selected.Id);
        ImGui.Separator();
    }

    public static void DrawComponents(GameObject selected)
    {
        foreach (Component component in selected.Components.ToArray())
        {
            DrawComponent(component);
        }
    }

    public static void DrawComponent(Component component)
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

    public static void DrawFooter(GameObject selected)
    {
        if (ImGui.Button("Add Component"))
        {
            ImGui.OpenPopup("AddComponentPopup");
        }

        if (ImGui.BeginPopup("AddComponentPopup"))
        {
            Type[] availableComponents = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type =>
                    type is { IsClass: true, IsAbstract: false } &&
                    typeof(Component).IsAssignableFrom(type)).ToArray();
            foreach (Type type in availableComponents)
            {
                if (!ComponentRules.CanBeAdded(type))
                    continue;
                if (ImGui.MenuItem(type.Name))
                {
                    selected.AddComponent(type);
                }
            }

            ImGui.EndPopup();
        }
    }
}
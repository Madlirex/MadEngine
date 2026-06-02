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
        if (ImGui.Button("Add Component"))
        {
            ImGui.OpenPopup("AddComponentPopup");
        }

        if (ImGui.BeginPopup("AddComponentPopup"))
        {
            Type[] availableComponents = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type =>
                    type.IsClass &&
                    !type.IsAbstract &&
                    typeof(Component).IsAssignableFrom(type)).ToArray();
            foreach (Type type in availableComponents)
            {
                if (!typeof(Component).IsAssignableFrom(type))
                    continue;
                if (ImGui.MenuItem(type.Name))
                {
                    Console.WriteLine(type.FullName);
                    selected.AddComponent((Component)Activator.CreateInstance((type))!);
                }
            }

            ImGui.EndPopup();
        }
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
        foreach (Component component in selected.Components)
        {
            DrawComponent(component);
        }
    }

    public static void DrawComponent(Component component)
    {
        string name = component.GetType().Name;
        if (ImGui.CollapsingHeader(name, ImGuiTreeNodeFlags.DefaultOpen))
        {
            foreach (FieldInfo field in component.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                if (FieldDrawerRegistry.TryGetDrawer(field.FieldType, out FieldDrawer drawer))
                {
                    drawer.Draw(component, field, component);
                }
            }
            ImGui.Separator();
        }
    }
}
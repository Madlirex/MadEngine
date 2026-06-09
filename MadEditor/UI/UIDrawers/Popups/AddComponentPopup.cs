using ImGuiNET;
using MadEngine.Core;

namespace MadEditor;

public class AddComponentPopup : Popup
{
    public override string Name => "AddComponentPopup";

    protected override void Body(EditorUIContext context)
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
                context.Selected!.AddComponent(type);
            }
        }
    }
}
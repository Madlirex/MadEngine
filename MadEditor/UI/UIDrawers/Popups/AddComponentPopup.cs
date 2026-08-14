using ImGuiNET;
using MadEngine.Core;

namespace MadEditor;

public class AddComponentPopup : Popup
{
    public override string Name => "AddComponentPopup";

    protected override void Body(EditorUIContext context)
    {
        if(context.Selected is not GameObject go) return;
        
        Type[] availableComponents = ScriptDomain.GetTypesImplementing(typeof(Component));
        foreach (Type type in availableComponents)
        {
            if (!ComponentRules.CanBeAdded(type))
                continue;
            if (ImGui.MenuItem(type.Name))
            {
                Component? comp = go.AddComponent(type);
                comp?.EditorStart();
            }
        }
    }
}
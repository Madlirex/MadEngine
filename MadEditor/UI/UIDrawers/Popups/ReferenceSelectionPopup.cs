using ImGuiNET;
using MadEngine.Core;

namespace MadEditor;

public class ReferenceSelectionPopup : Popup
{
    public override string Name => "ReferenceSelectionPopup";
    public Type Type = typeof(MadObject);
    
    public Action<MadObject?>? OnObjectSelected { get; set; }
    
    protected override void Body(EditorUIContext context)
    {
        if (ImGui.Button("None"))
        {
            OnObjectSelected?.Invoke(null);
            ImGui.CloseCurrentPopup();
            return;
        }

        ImGui.Separator();
        
        foreach (var obj in AssetRegistry.GetObjectsImplementing(Type))
        {
            if (ImGui.Selectable(obj.ToString(), false))
            {
                OnObjectSelected?.Invoke(obj);
                ImGui.CloseCurrentPopup();
                return;
            }
        }
    }
}
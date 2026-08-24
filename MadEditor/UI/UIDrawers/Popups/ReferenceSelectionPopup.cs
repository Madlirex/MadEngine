using ImGuiNET;
using MadEngine.Core;

namespace MadEditor;

public class ReferenceSelectionPopup : Popup
{
    public override string Name => "ReferenceSelectionPopup";
    public Type Type = typeof(MadObject);
    public MadObject? Selected;
    
    public Action<MadObject?>? OnObjectSelected { get; set; }
    
    protected override void Body(EditorUIContext context)
    {
        if (ImGui.Selectable("None", Selected == null))
        {
            OnObjectSelected?.Invoke(null);
            ImGui.CloseCurrentPopup();
            return;
        }

        ImGui.Separator();
        
        foreach (var obj in AssetRegistry.GetObjectsImplementing(Type))
        {
            if (!ImGui.Selectable(obj.ToString(), Selected == obj)) continue;
            
            OnObjectSelected?.Invoke(obj);
            ImGui.CloseCurrentPopup();
            return;
        }
    }
}
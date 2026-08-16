using ImGuiNET;

namespace MadEditor;

public static class DragPayload
{
    public const string GameObjectType = "GAMEOBJECT";
    
    public static unsafe void DragSource(string type, Guid id, string tooltipText)
    {
        if (ImGui.BeginDragDropSource())
        {
            ImGui.SetDragDropPayload(type, (nint)(&id), (uint)sizeof(Guid));
            
            ImGui.Text(tooltipText);
            
            ImGui.EndDragDropSource();
        }
    }
    
    public static unsafe bool DropTarget(string type, Action<Guid> onDropAccepted)
    {
        if (ImGui.BeginDragDropTarget())
        {
            ImGuiPayloadPtr payload = ImGui.AcceptDragDropPayload(type);

            if (payload.NativePtr != null)
            {
                Guid droppedGuid = *(Guid*)payload.Data;
                
                onDropAccepted?.Invoke(droppedGuid);
                
                ImGui.EndDragDropTarget();
                return true;
            }

            ImGui.EndDragDropTarget();
        }
        return false;
    }
}
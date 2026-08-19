using ImGuiNET;

namespace MadEditor;

public static class DragPayload
{
    public const string MadObjectType = "GAMEOBJECT";

    public static unsafe void DragSource(
        string type,
        Guid id,
        string tooltipText)
    {
        if (ImGui.BeginDragDropSource())
        {
            Console.WriteLine($"DRAG SOURCE TYPE: '{type}'");
            Console.WriteLine($"DRAG SOURCE ID: {id}");

            ImGui.SetDragDropPayload(
                type,
                (nint)(&id),
                (uint)sizeof(Guid));

            ImGui.Text(tooltipText);

            ImGui.EndDragDropSource();
        }
    }

    public static unsafe bool DropTarget(
        string type,
        Action<Guid> onDropAccepted)
    {
        if (!ImGui.BeginDragDropTarget())
            return false;

        Console.WriteLine($"DROP TARGET TYPE: '{type}'");

        var payload = ImGui.AcceptDragDropPayload(type);

        

        

        
        Guid droppedGuid = *(Guid*)payload.Data;

        Console.WriteLine($"DROPPED: {droppedGuid}");

        onDropAccepted(droppedGuid);

        ImGui.EndDragDropTarget();
        return true;
        

        ImGui.EndDragDropTarget();
        return false;
    }
}
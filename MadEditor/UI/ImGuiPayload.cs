using ImGuiNET;

namespace MadEditor;

public static class DragDrop
{
    private const string DummyPayloadId = "ENGINE_DND_TYPE";
    
    public static object? CurrentPayload { get; private set; }
    
    public static bool BeginSource<T>(T payload, string previewText) where T : class
    {
        if (ImGui.BeginDragDropSource())
        {
            CurrentPayload = payload;

            ImGui.SetDragDropPayload(DummyPayloadId, (nint)null, 0);
            
            ImGui.Text(previewText);
            ImGui.EndDragDropSource();
            return true;
        }
        return false;
    }
    
    public static bool TryAcceptTarget<T>(out T? result) where T : class
    {
        result = null;
        if (ImGui.BeginDragDropTarget())
        {
            ImGui.AcceptDragDropPayload(DummyPayloadId);
            
            if (ImGui.IsMouseReleased(ImGuiMouseButton.Left) && CurrentPayload is T typedPayload)
            {
                result = typedPayload;
                CurrentPayload = null;
                ImGui.EndDragDropTarget();
                return true;
            }

            ImGui.EndDragDropTarget();
        }

        return false;
    }

    public static bool TryAcceptTarget(Type type, out object? result)
    {
        result = null;

        if (ImGui.BeginDragDropTarget())
        {
            ImGui.AcceptDragDropPayload(DummyPayloadId);
            
            if (ImGui.IsMouseReleased(ImGuiMouseButton.Left) && CurrentPayload != null && type.IsInstanceOfType(CurrentPayload))
            {
                result = CurrentPayload;
                CurrentPayload = null;
                ImGui.EndDragDropTarget();
                return true;
            }

            ImGui.EndDragDropTarget();
        }

        return false;
    }
    
    public static void UpdateFrameState()
    {
        if (CurrentPayload != null && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
        {
            CurrentPayload = null;
        }
    }
}

using ImGuiNET;

namespace MadEditor;

public static class ImGuiPayload
{
    public const string Name = "GAMEOBJECT";

    public static unsafe void Set(Guid id)
    {
        ImGui.SetDragDropPayload(Name, (nint)(&id), (uint)sizeof(Guid));
    }

    public static unsafe Guid DataToGuid(nint data)
    {
        return *(Guid*)data;
    }

    public static unsafe bool TryGetData(out nint? data)
    {
        ImGuiPayloadPtr payload = ImGui.AcceptDragDropPayload(Name);

        if (payload.NativePtr != null)
        {
            data = payload.Data;
            return true;
        }

        data = null;
        return false;
    }
}
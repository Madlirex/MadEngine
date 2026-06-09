using ImGuiNET;

namespace MadEditor;

public abstract class Popup
{
    public Guid Id = Guid.NewGuid();
    public virtual string Name => "Popup";

    public string FullName => $"{Name} ({Id})";

    public void Open()
    {
        ImGui.OpenPopup(FullName);
    }

    public void Draw(EditorUIContext context)
    {
        if (ImGui.BeginPopup(FullName))
        {
            Body(context);
            ImGui.EndPopup();
        }
    }

    protected abstract void Body(EditorUIContext context);
}
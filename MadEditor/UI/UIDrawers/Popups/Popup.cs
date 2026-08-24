using ImGuiNET;

namespace MadEditor;

public abstract class Popup
{
    public Guid Id = Guid.NewGuid();
    public virtual string Name => "Popup";

    public string FullName => $"{Name} ({Id})";

    private bool _isOpen;

    public void Open()
    {
        _isOpen = true;
        ImGui.OpenPopup(FullName);
        
        OnOpen();
    }

    public void Close()
    {
        _isOpen = false;

        OnClose();
    }

    public virtual void OnOpen() {}
    public virtual void OnClose() {}

    public void Draw(EditorUIContext context)
    {
        if (!_isOpen) return;

        if (ImGui.BeginPopup(FullName))
        {
            Body(context);
            
            ImGui.EndPopup(); 
        }
        else
        {
            Close();
        }
    }

    protected abstract void Body(EditorUIContext context);
}
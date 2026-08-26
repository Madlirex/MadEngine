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

    protected virtual void OnOpen() {}
    protected virtual void OnClose() {}

    public void Draw(EditorUIContext context)
    {
        if (!_isOpen) return;

        if (ImGui.BeginPopup(FullName))
        {
            _isOpen = true;
            Body(context);
            
            ImGui.EndPopup(); 
        }
        else
        {
            if(!_isOpen) Close();
        }
    }

    protected abstract void Body(EditorUIContext context);
}
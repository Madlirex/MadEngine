using ImGuiNET;

namespace MadEditor;

public abstract class Popup
{
    public Guid Id = Guid.NewGuid();
    public virtual string Name => "Popup";

    public string FullName => $"{Name} ({Id})";

    private bool _isOpen;
    private bool _shouldOpen;

    public void Open()
    {
        _isOpen = true;
        _shouldOpen = true;

        PopupManager.Add(this);
        
        OnOpen();
    }

    public void Close()
    {
        _isOpen = false;
        _shouldOpen = false;
        PopupManager.Remove(this);

        OnClose();
    }

    protected virtual void OnOpen() {}
    protected virtual void OnClose() {}

    public void Draw(EditorUIContext context)
    {
        if (!_isOpen) return;
        
        if (_shouldOpen)
        {
            ImGui.OpenPopup(FullName);
            _shouldOpen = false; 
        }
        
        if (ImGui.BeginPopup(FullName))
        {
            _isOpen = true;
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
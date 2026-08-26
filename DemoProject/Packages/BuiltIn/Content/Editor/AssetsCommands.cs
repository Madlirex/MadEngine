using ImGuiNET;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;

namespace MadEditor.Commands;

internal class RenamePopup : Popup
{
    private bool _first = true;
    private string _newName = "";
    
    protected override void Body(EditorUIContext context)
    {
        if (_first) SetName(context.Selected!.Name);
        if (ImGui.InputText("Name", ref _newName, 256, ImGuiInputTextFlags.EnterReturnsTrue))
        {
            SubmitRename(context);
        }
    }

    private void SetName(string name)
    {
        _newName = name;
        _first = false;
    }
    
    private void SubmitRename(EditorUIContext context)
    {
        if (string.IsNullOrEmpty(_newName)) return;

        if (context.Selected is not Asset asset) return;
        string oldPath = asset.AbsolutePath;
        asset.Name = _newName;
        File.Move(oldPath, asset.AbsolutePath);
        File.Move(oldPath + ".meta", asset.AbsolutePath + ".meta");
        Close();
    }
    
}

public class RenameAssetCommand : PopupCommand<Asset>
{
    private readonly RenamePopup _renamePopup = new();
    public override string Path => "Rename";
    public override void Execute(Asset target)
    {
        _renamePopup.Open();
    }
}

public class DeleteAssetCommand : PopupCommand<Asset>
{
    public override string Path => "Delete";
    public override void Execute(Asset target)
    {
        File.Delete(target.AbsolutePath);
        File.Delete(target.AbsolutePath + ".meta");
        target.Destroy();
    }
}
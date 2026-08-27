using ImGuiNET;
using MadEngine;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;

namespace MadEditor.Commands;

public class CreateTextureCommand : PopupCommand<Asset>
{
    public override string Path => "Create/Texture";
    public override void Execute(Asset target)
    {
        AssetManager.SaveAsset(new Texture2D());
    }
}

public class CreateMaterialCommand : PopupCommand<Asset>
{
    public override string Path => "Create/Material";
    public override void Execute(Asset target)
    {
        AssetManager.SaveAsset(new Material());
    }
}

public class CreateShaderCommand : PopupCommand<Asset>
{
    public override string Path => "Create/Shader";
    public override void Execute(Asset target)
    {
        AssetManager.SaveAsset(new Shader());
    }
}

public class CreateMeshCommand : PopupCommand<Asset>
{
    public override string Path => "Create/Mesh";
    public override void Execute(Asset target)
    {
        AssetManager.SaveAsset(new Mesh());
    }
}

public class CreateSceneCommand : PopupCommand<Asset>
{
    public override string Path => "Create/Scene";
    public override void Execute(Asset target)
    {
        AssetManager.SaveAsset(new Scene());
    }
}

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
        AssetManager.SaveAsset(asset);
        Close();
    }
    
}

public class RenameAssetCommand : PopupCommand<Asset>
{
    private readonly RenamePopup _renamePopup = new();
    public override Type[] ExcludingTypes => [typeof(NoneAsset)];
    public override string Path => "Rename";
    public override void Execute(Asset target)
    {
        _renamePopup.Open();
    }
}

public class DeleteAssetCommand : PopupCommand<Asset>
{
    public override string Path => "Delete";
    public override Type[] ExcludingTypes => [typeof(NoneAsset)];
    public override void Execute(Asset target)
    {
        File.Delete(target.AbsolutePath);
        File.Delete(target.AbsolutePath + ".meta");
        target.Destroy();
    }
}
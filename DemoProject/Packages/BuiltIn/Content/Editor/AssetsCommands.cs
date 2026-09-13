using ImGuiNET;
using MadEngine;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;

namespace MadEditor.Commands;

[CategoryOrder("Create", 200)]
public class CreateTextureCommand : PopupCommand<Asset>
{
    public override string Path => "Create/Texture";
    public override void Execute(Asset target)
    {
        AssetManager.CreateAsset<Texture2D>(target.FullDir);
    }
}

public class CreateMaterialCommand : PopupCommand<Asset>
{
    public override string Path => "Create/Material";
    public override void Execute(Asset target)
    {
        AssetManager.CreateAsset<Material>(target.FullDir);
    }
}

public class CreateShaderCommand : PopupCommand<Asset>
{
    public override string Path => "Create/Shader";
    public override void Execute(Asset target)
    {
        AssetManager.CreateAsset<Shader>(target.FullDir);
    }
}

public class CreateMeshCommand : PopupCommand<Asset>
{
    public override string Path => "Create/Mesh";
    public override void Execute(Asset target)
    {
        AssetManager.CreateAsset<Mesh>(target.FullDir);
    }
}

public class CreateSceneCommand : PopupCommand<Asset>
{
    public override string Path => "Create/Scene";
    public override void Execute(Asset target)
    {
        AssetManager.CreateAsset<Scene>(target.FullDir);
    }
}

[Order(-600)]
public class CreateFolderCommand : PopupCommand<Asset>
{
    public override string Path => "Create/Folder";
    public override void Execute(Asset target)
    {
        AssetManager.CreateAsset<FolderAsset>(target.FullDir);
    }
}

internal class RenamePopup : Popup
{
    private bool _first = true;
    private string _newName = "";
    
    protected override void Body(EditorUIContext context)
    {
        if (_first) SetName(context.RightClicked!.Name);
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

        if (context.RightClicked is not Asset asset) return;
        
        string oldPath = asset.AbsolutePath;
        asset.Name = _newName;
        string newPath = asset.AbsolutePath;
        
        if (oldPath.Equals(newPath, StringComparison.Ordinal))
        {
            Close();
            return;
        }
        
        if (asset is FolderAsset || Directory.Exists(oldPath))
        {
            Directory.Move(oldPath, newPath);
        }
        else if (File.Exists(oldPath))
        {
            File.Move(oldPath, newPath);
        }
        
        string oldMetaPath = oldPath + ".meta";
        string newMetaPath = newPath + ".meta";
    
        if (File.Exists(oldMetaPath))
        {
            File.Move(oldMetaPath, newMetaPath);
        }
        
        AssetManager.SaveAsset(asset);
        
        _first = true;
        Close();
    }
    
}

[Order(600)]
public class RenameAssetCommand : PopupCommand<Asset>
{
    private readonly RenamePopup _renamePopup = new();
    public override Type[] ExcludingTypes => [typeof(NoneAsset)];
    public override string Path => "Rename";
    public override void Execute(Asset target)
    {
        EditorUI.UiContext.RightClicked = target;
        _renamePopup.Open();
    }
}

[Order(1500)]
public class DeleteAssetCommand : PopupCommand<Asset>
{
    public override string Path => "Delete";
    public override Type[] ExcludingTypes => [typeof(NoneAsset)];
    public override void Execute(Asset target)
    {
        string targetPath = target.AbsolutePath;
        if (!System.IO.Path.Exists(targetPath)) return;
        
        if (target is FolderAsset || Directory.Exists(targetPath))
        {
            string folderPrefix = targetPath.EndsWith(System.IO.Path.DirectorySeparatorChar) ? targetPath : targetPath + System.IO.Path.DirectorySeparatorChar;
            
            var subAssets = AssetRegistry.Assets
                .Where(asset => asset.AbsolutePath.StartsWith(folderPrefix, StringComparison.Ordinal))
                .ToArray();

            foreach (var subAsset in subAssets)
            {
                if (File.Exists(subAsset.AbsolutePath + ".meta"))
                {
                    File.Delete(subAsset.AbsolutePath + ".meta");
                }
                
                subAsset.Destroy(); 
            }
            
            Directory.Delete(targetPath, recursive: true);
        }
        else
        {
            File.Delete(targetPath);
        }
        
        if(File.Exists(targetPath + ".meta"))
            File.Delete(targetPath + ".meta");
        
        target.Destroy();
    }
}
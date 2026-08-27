using ImGuiNET;
using System.Numerics;
using MadEngine.Core;

namespace MadEditor;

[CustomName("Project")]
public class ProjectPanelDrawer : PanelDrawer
{
    private readonly ProjectPopup _projectPopup = new();
    private readonly NoneAsset _noneAsset = new();
    
    public override PanelRegion PanelRegion { get; set; } = PanelRegion.Bottom;

    private readonly string _rootAssetsPath = Application.Directory;
    
    private string _selectedDirectory = "";

    public ProjectPanelDrawer()
    {
        if (Directory.Exists(_rootAssetsPath))
        {
            _selectedDirectory = _rootAssetsPath;
        }
    }

    public override void Draw(EditorUIContext context)
    {
        
        if (!Directory.Exists(_rootAssetsPath))
        {
            ImGui.TextColored(new Vector4(1, 0, 0, 1), $"Assets directory not found at: {_rootAssetsPath}");
            return;
        }
        
        ImGui.Columns(2, "ProjectPanelSplit", true);
        
        ImGui.SetColumnWidth(0, 200.0f); 
        
        ImGui.BeginChild("DirectoryTreeChild");
        RenderDirectoryNode(new DirectoryInfo(_rootAssetsPath));
        ImGui.EndChild();
        
        ImGui.NextColumn();
        
        ImGui.BeginChild("FolderContentChild");
        RenderFolderContents(context);
        
        if (ImGuiEx.IsClickedOutside(ImGuiMouseButton.Right))
        {
            context.RightClicked = _noneAsset;
            _projectPopup.Open();
        }
        _projectPopup.Draw(context);
        
        ImGui.EndChild();
        
        ImGui.Columns(1);
    }
    
    private void RenderDirectoryNode(DirectoryInfo directory)
    {
        ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick;

        if (directory.Name is "obj" or "bin") return;
        
        if (_selectedDirectory == directory.FullName)
            flags |= ImGuiTreeNodeFlags.Selected;
        
        if (directory.GetDirectories().Length == 0)
            flags |= ImGuiTreeNodeFlags.Leaf;

        bool isOpen = ImGui.TreeNodeEx($"{directory.Name}##Tree", flags);
        
        if (ImGui.IsItemClicked())
        {
            _selectedDirectory = directory.FullName;
        }

        if (!isOpen) return;
        foreach (var subDir in directory.GetDirectories())
        {
            RenderDirectoryNode(subDir);
        }
        ImGui.TreePop();
    }
    
    private void RenderFolderContents(EditorUIContext context)
    {
        if (string.IsNullOrEmpty(_selectedDirectory) || !Directory.Exists(_selectedDirectory))
        {
            ImGui.Text("Select a folder to view contents.");
            return;
        }

        DirectoryInfo currentDir = new DirectoryInfo(_selectedDirectory);
        
        foreach (var dir in currentDir.GetDirectories())
        {
            if (dir.Name is "obj" or "bin") continue;
            ImGui.Selectable($"? {dir.Name}", false);
            
            if (ImGui.IsItemHovered() && ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
            {
                _selectedDirectory = dir.FullName;
                return;
            }
        }
        
        foreach (var file in currentDir.GetFiles())
        {
            if (file.Extension is ".meta" or ".csproj") continue;
            string absolutePath = file.FullName; 
            
            Asset? mappedAsset = AssetRegistry.Assets.FirstOrDefault(a => a.AbsolutePath == absolutePath);

            string fileLabel = $"? {file.Name}";
            if (mappedAsset != null)
            {
                fileLabel = $"? {mappedAsset.Name} ({mappedAsset.GetType().GetCustomName()})";
                
            }
            
            if (ImGui.Selectable(fileLabel, false))
            {
                if(mappedAsset != null)
                {
                    context.Selected = mappedAsset;
                }
            }

            if (mappedAsset == null) continue;
            
            DragDrop.BeginSource(mappedAsset, mappedAsset.Name);

            if (!ImGuiEx.IsClicked(ImGuiMouseButton.Right)) continue;
            context.RightClicked = mappedAsset;
            _projectPopup.Open();
        }
    }
}

public class NoneAsset : Asset;
using System.Numerics;
using ImGuiNET;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;

namespace MadEditor;

public class HierarchyDrawer : IPanelDrawer
{
    public string Name => "Hierarchy";
    public PanelRegion PanelRegion { get; set; } = PanelRegion.Left;
    public HierarchyPopup HierarchyPopup = new();
    public void Draw(EditorUIContext context)
    {
        Scene scene = SceneManager.ActiveScene;

        ImGui.PushID(scene.Name);

        ImGuiTreeNodeFlags sceneFlags =
            ImGuiTreeNodeFlags.DefaultOpen |
            ImGuiTreeNodeFlags.SpanFullWidth |
            ImGuiTreeNodeFlags.FramePadding;

        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(0.7f, 0.85f, 1f, 1f));
        ImGui.PushStyleVar(ImGuiStyleVar.IndentSpacing, 4f);
        bool sceneOpen = ImGui.TreeNodeEx(scene.Name, sceneFlags);
        ImGui.PopStyleColor();
        
        CheckDragDrop(null);

        if (sceneOpen)
        {
            foreach (var root in scene.GameObjects.Where(go => go.Transform.Parent == null))
            {
                DrawNode(root, context);
            }

            ImGui.TreePop();
        }

        ImGui.PopID();
        ImGui.PopStyleVar();
    }

    private void DrawNode(GameObject root, EditorUIContext context)
    {
        bool isSelected = context.Selected == root;

        ImGuiTreeNodeFlags flags =
            ImGuiTreeNodeFlags.OpenOnArrow |
            ImGuiTreeNodeFlags.SpanFullWidth |
            (isSelected ? ImGuiTreeNodeFlags.Selected : 0);

        bool hasChildren = root.Transform.Children.Count > 0;

        if (!hasChildren)
            flags |= ImGuiTreeNodeFlags.Leaf;

        string label = $"{root.Name}##{root.Id}";

        bool open = ImGui.TreeNodeEx(label, flags);
        
        if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
            context.Selected = root;

        if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
        {
            context.Selected = root;
            HierarchyPopup.Open();
        }
        HierarchyPopup.Draw(context);
        
        if (ImGui.BeginDragDropSource())
        {
            ImGuiPayload.Set(root.Id);
            ImGui.Text(root.Name);
            ImGui.EndDragDropSource();
        }
        
        CheckDragDrop(root);
        
        if (open)
        {
            if (hasChildren)
            {
                foreach (var child in root.Transform.Children)
                {
                    DrawNode(child.GameObject, context);
                }
            }

            ImGui.TreePop();
        }
    }
    
    

    public void CheckDragDrop(GameObject? root)
    {
        if (ImGui.BeginDragDropTarget())
        {
            if (ImGuiPayload.TryGetData(out nint? data))
            {
                Guid draggedId = ImGuiPayload.DataToGuid((IntPtr)data!);
                Scene scene = SceneManager.ActiveScene;
                GameObject? dragged = scene.GameObjects.FirstOrDefault(x => x.Id == draggedId);
                
                if (dragged != null && dragged != root)
                {
                    if(root != null && !dragged.Transform.IsDescendantOf(root.Transform))
                        dragged.Transform.Parent = root.Transform;
                    else if(root == null)
                        dragged.Transform.Parent = null;
                }
            }
            ImGui.EndDragDropTarget();
        }
    }
}
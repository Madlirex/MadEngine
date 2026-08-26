using System.Numerics;
using ImGuiNET;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;

namespace MadEditor;

[CustomName("Hierarchy")]
public class HierarchyDrawer : PanelDrawer
{
    public override PanelRegion PanelRegion { get; set; } = PanelRegion.Left;
    public HierarchyPopup HierarchyPopup = new();
    public override void Draw(EditorUIContext context)
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

        if (ImGuiEx.IsClicked(ImGuiMouseButton.Left))
        {
            context.Selected = scene;
        }
        
        if (ImGuiEx.IsClicked(ImGuiMouseButton.Right))
        {
            context.RightClicked = scene;
            HierarchyPopup.Open();
        }

        DragDrop.BeginSource(scene, scene.Name);

        if (DragDrop.TryAcceptTarget<GameObject>(out var draggedNode))
        {
            draggedNode!.Transform.Parent = null;
        }

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

        string label = $"{root.Name}##{root.Guid}";

        bool open = ImGui.TreeNodeEx(label, flags);

        if (ImGuiEx.IsClicked(ImGuiMouseButton.Left))
        {
            context.Selected = root;
        }

        if (ImGuiEx.IsClicked(ImGuiMouseButton.Right))
        {
            context.RightClicked = root;
            HierarchyPopup.Open();
        }
        HierarchyPopup.Draw(context);
        
        DragDrop.BeginSource(root, root.Name);
        
        if (DragDrop.TryAcceptTarget<GameObject>(out var draggedNode))
        {
            draggedNode!.Transform.Parent = root.Transform;
        }
        
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
}
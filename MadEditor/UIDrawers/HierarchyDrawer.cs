using System.Numerics;
using ImGuiNET;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;

namespace MadEditor;

public class HierarchyDrawer : IPanelDrawer
{
    public string Name => "Hierarchy";
    public PanelRegion PanelRegion { get; set; } = PanelRegion.Left;
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

        // selection
        if (ImGui.IsItemClicked())
            context.Selected = root;

        // drag source
        if (ImGui.BeginDragDropSource())
        {
            unsafe
            {
                Guid id = root.Id;
                ImGui.SetDragDropPayload("GAMEOBJECT", (nint)(&id), (uint)sizeof(Guid));
            }

            ImGui.Text(root.Name);
            ImGui.EndDragDropSource();
        }

        // drop target
        if (ImGui.BeginDragDropTarget())
        {
            unsafe
            {
                var payload = ImGui.AcceptDragDropPayload("GAMEOBJECT");

                if (payload.NativePtr != null)
                {
                    Guid draggedId = *(Guid*)payload.Data;

                    var scene = SceneManager.ActiveScene;

                    var dragged = scene.GameObjects.FirstOrDefault(x => x.Id == draggedId);

                    if (dragged != null &&
                        dragged != root &&
                        !dragged.Transform.IsDescendantOf(root.Transform))
                    {
                        dragged.Transform.Parent = root.Transform;
                    }
                }

                ImGui.EndDragDropTarget();
            }
        }

        // IMPORTANT: TreePop ONLY if not leaf
        if (open)
        {
            if (hasChildren)
            {
                foreach (var child in root.Transform.Children)
                    DrawNode(child.GameObject, context);
            }

            ImGui.TreePop(); // ALWAYS paired with open == true
        }
    }
}
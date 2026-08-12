using System.Numerics;
using ImGuiNET;

namespace MadEditor;

public enum PanelRegion
{
    Left,
    Center,
    Right,
    Bottom,
    Floating
}

public static class PanelLayoutManager
{
    private static readonly Dictionary<PanelRegion, uint> DockIDs = new();
    private static bool _isInitialized;

    public static uint GetDockId(PanelRegion region, uint mainDockSpaceId)
    {
        return DockIDs.TryGetValue(region, out uint id) ? id : mainDockSpaceId;
    }

    public static uint DrawMainDockSpace()
    {
        var viewport = ImGui.GetMainViewport();
        
        ImGui.SetNextWindowPos(viewport.WorkPos);
        ImGui.SetNextWindowSize(viewport.WorkSize);
        ImGui.SetNextWindowViewport(viewport.ID);

        ImGuiWindowFlags hostWindowFlags = ImGuiWindowFlags.NoTitleBar | 
                                          ImGuiWindowFlags.NoCollapse | 
                                          ImGuiWindowFlags.NoResize | 
                                          ImGuiWindowFlags.NoMove | 
                                          ImGuiWindowFlags.NoBringToFrontOnFocus | 
                                          ImGuiWindowFlags.NoNavFocus |
                                          ImGuiWindowFlags.NoBackground;

        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0.0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
        
        ImGui.Begin("MainDockSpaceHost", hostWindowFlags);
        ImGui.PopStyleVar(3);
        
        uint dockspaceId = ImGui.GetID("EditorCentralDockSpace");
        ImGui.DockSpace(dockspaceId, Vector2.Zero, ImGuiDockNodeFlags.None);
        
        if (!_isInitialized)
        {
            BuildDefaultLayout(dockspaceId);
            _isInitialized = true;
        }

        ImGui.End();
        return dockspaceId;
    }

    private static void BuildDefaultLayout(uint mainDockSpaceId)
    {
        ImGuiInternal.DockBuilderRemoveNode(mainDockSpaceId);
        ImGuiInternal.DockBuilderAddNode(mainDockSpaceId, ImGuiDockNodeFlags.None);
        ImGuiInternal.DockBuilderSetNodeSize(mainDockSpaceId, ImGui.GetMainViewport().WorkSize);

        uint centralNodeId = mainDockSpaceId;
        
        uint leftDockId = ImGuiInternal.DockBuilderSplitNode(centralNodeId, ImGuiDir.Left, 0.18f, out _, out centralNodeId);
        
        uint rightDockId = ImGuiInternal.DockBuilderSplitNode(centralNodeId, ImGuiDir.Right, 0.22f, out _, out centralNodeId);
        
        uint bottomDockId = ImGuiInternal.DockBuilderSplitNode(centralNodeId, ImGuiDir.Down, 0.20f, out _, out uint centerDockId);

        
        DockIDs[PanelRegion.Left] = leftDockId;
        DockIDs[PanelRegion.Right] = rightDockId;
        DockIDs[PanelRegion.Bottom] = bottomDockId;
        DockIDs[PanelRegion.Center] = centerDockId;

        ImGuiInternal.DockBuilderFinish(mainDockSpaceId);
    }
}

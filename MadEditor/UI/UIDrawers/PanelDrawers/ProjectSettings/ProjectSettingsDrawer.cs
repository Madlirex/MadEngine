using System.Reflection;
using ImGuiNET;
using MadEngine.Core;

namespace MadEditor;

[CustomName("Project Settings")]
public class ProjectSettingsDrawer : PanelDrawer
{
    private IProjectSettingsTab? SelectedTab => _hierarchyTreeRenderer.SelectedInstance;
    private IProjectSettingsTab? _lastSelectedTab;
    private HierarchyTreeRenderer<IProjectSettingsTab> _hierarchyTreeRenderer;

    public ProjectSettingsDrawer()
    {
        _hierarchyTreeRenderer = new HierarchyTreeRenderer<IProjectSettingsTab>(ProjectSettingsTabsRegistry.LoadCategoryWeights());
        RebuildTree();
    }
    
    public override void Draw(EditorUIContext context)
    {
        if (!ImGui.BeginTable($"PackageManagerLayout##{Guid}", 2,
                ImGuiTableFlags.Resizable | ImGuiTableFlags.BordersInnerV)) return;
        ImGui.TableSetupColumn("Left", ImGuiTableColumnFlags.WidthStretch, 0.3f);
        ImGui.TableSetupColumn("Right", ImGuiTableColumnFlags.WidthStretch, 0.7f);

        if(ImGui.TableNextColumn())
            DrawLeftPanel();
            
        if(ImGui.TableNextColumn())
            DrawRightPanel(context);
            
        ImGui.EndTable();
    }

    public void DrawLeftPanel()
    {
        ImGui.TextDisabled("Project Settings");
        ImGui.Separator();
        
        _hierarchyTreeRenderer.Draw();

        if (_lastSelectedTab == SelectedTab) return;
        if(_lastSelectedTab != null)
            ProjectSettingsManager.SaveSettings(_lastSelectedTab.Settings);
        _lastSelectedTab = SelectedTab;
    }

    public void DrawRightPanel(EditorUIContext context)
    {
        ImGui.BeginChild("SettingsContent", new System.Numerics.Vector2(0, 0), ImGuiChildFlags.None);
        {
            if (SelectedTab != null)
            {
                ImGui.TextDisabled($"{string.Join(" > ", SelectedTab.Path.Split('/'))}");
                ImGui.Separator();
                ImGui.Spacing();
                
                SelectedTab.Draw(context);
            }
            else
            {
                ImGui.TextDisabled("No category selected.\nPlease select a category.");
            }
        }
        ImGui.EndChild();
    }
    
    private void RebuildTree()
    {
        var menuData = ProjectSettingsTabsRegistry.Tabs.Select(tab => {
            var orderAttr = tab.GetType().GetCustomAttribute<OrderAttribute>();
            return new TreeNodeData<IProjectSettingsTab>($"{tab.Path}##{tab.Guid}", orderAttr?.Order ?? 0, tab);
        });
        
        _hierarchyTreeRenderer.RegenerateTree(menuData);
    }
}
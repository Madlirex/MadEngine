using ImGuiNET;
using MadEngine.Core;

namespace MadEditor;

[CustomName("Project Settings")]
public class ProjectSettingsDrawer : PanelDrawer
{
    private ProjectSettingsTab? _selectedTab;
    
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
        
        ImGui.BeginChild("SettingsSidebar", new System.Numerics.Vector2(200, 0), ImGuiChildFlags.None);
        {
            foreach (var tab in ProjectSettingsTabsRegistry.Tabs)
            {
                bool isSelected = _selectedTab == tab;
                if (ImGui.Selectable(tab.Path, isSelected))
                {
                    _selectedTab = tab;
                }
            }
        }
        
        ImGui.EndChild();
    }

    public void DrawRightPanel(EditorUIContext context)
    {
        ImGui.BeginChild("SettingsContent", new System.Numerics.Vector2(0, 0), ImGuiChildFlags.None);
        {
            if (_selectedTab != null)
            {
                ImGui.TextDisabled($"Project Settings > {_selectedTab.Path}");
                ImGui.Separator();
                ImGui.Spacing();
                
                _selectedTab.Draw(context);
            }
            else
            {
                ImGui.TextDisabled("No category selected.\nPlease select a category.");
            }
        }
        ImGui.EndChild();
    }
}
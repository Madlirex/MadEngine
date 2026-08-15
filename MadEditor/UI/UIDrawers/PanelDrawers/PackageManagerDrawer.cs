using System.Numerics;
using ImGuiNET;
using MadEditor.PackageManagement;
using MadEngine.Core;

namespace MadEditor;

[CustomName("Package Manager")]
public class PackageManagerDrawer : PanelDrawer
{
    public override PanelRegion PanelRegion { get; set; }
    
    private Guid? _selectedPackageGuid;
    
    public override void Draw(EditorUIContext context)
    {
        if (ImGui.BeginTable($"PackageManagerLayout##{Guid}", 2,
                ImGuiTableFlags.Resizable | ImGuiTableFlags.BordersInnerV))
        {
            ImGui.TableSetupColumn("Left", ImGuiTableColumnFlags.WidthStretch, 0.3f);
            ImGui.TableSetupColumn("Right", ImGuiTableColumnFlags.WidthStretch, 0.7f);

            ImGui.TableNextColumn();
            DrawLeftPanel();
            
            ImGui.TableNextColumn();
            DrawRightPanel();
            
            ImGui.EndTable();
        }
    }

    private void DrawLeftPanel()
    {
        ImGui.TextDisabled("Packages");
        ImGui.Separator();

        if (ImGui.BeginChild("PackageListRegion", new Vector2(0, 0), ImGuiChildFlags.None))
        {
            if (ImGui.BeginTable("PackageListTable", 2, ImGuiTableFlags.NoBordersInBody))
            {
                ImGui.TableSetupColumn("NameColumn", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("ActionColumn", ImGuiTableColumnFlags.WidthFixed, 60.0f);

                foreach (var pair in PackageManager.PackagesMetas)
                {
                    PackageMeta meta = pair.Value;
                    bool isSelected = _selectedPackageGuid == meta.Guid;

                    ImGui.PushID(meta.Guid.ToString());
                    ImGui.TableNextRow(ImGuiTableRowFlags.None, ImGui.GetFrameHeight());
                    
                    ImGui.TableNextColumn();
                    
                    float itemHeight = ImGui.GetFrameHeight();

                    if (ImGui.Selectable(meta.Name, isSelected,
                            ImGuiSelectableFlags.SpanAllColumns | ImGuiSelectableFlags.AllowOverlap,
                            new Vector2(0, itemHeight)))
                    {
                        _selectedPackageGuid = meta.Guid;
                    }
                    
                    ImGui.TableNextColumn();

                    if (PackageManager.IsUpdate(meta.Guid))
                    {
                        if (ImGui.Button("Update"))
                        {
                            Console.WriteLine($"Updating {meta.Name}");
                        }
                    }

                    ImGui.PopID();
                }

                ImGui.EndTable();
            }

            ImGui.EndChild();
        }
    }

    private void DrawRightPanel()
    {
        if (_selectedPackageGuid is not { } guid)
        {
            ImGui.TextDisabled("No package selected.\nPlease select a package.");
            return;
        }
        
        PackageMeta? meta = PackageManager.PackagesMetas.GetValueOrDefault(guid);
        if (meta is null)
        {
            ImGui.TextDisabled("Package details unavailable.");
            return;
        }
        
        ImGui.SetWindowFontScale(1.3f);
        ImGui.Text(meta.Name);
        ImGui.SetWindowFontScale(1.0f);
        
        ImGui.TextDisabled($"Author: {meta.Author} from {meta.Company}");
        ImGui.TextDisabled($"Version: {meta.Version}");
        ImGui.Separator();
        
        ImGui.Spacing();
        ImGui.TextWrapped(meta.Description);
    }
}

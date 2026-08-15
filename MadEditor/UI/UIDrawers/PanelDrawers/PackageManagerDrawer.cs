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
                    
                    float itemHeight = ImGui.GetFrameHeight();
                    ImGui.TableNextRow(ImGuiTableRowFlags.None, itemHeight);
                    
                    ImGui.TableNextColumn();
                    
                    if (ImGui.Selectable($"##Selectable_{meta.Guid}", isSelected,
                            ImGuiSelectableFlags.SpanAllColumns | ImGuiSelectableFlags.AllowOverlap,
                            new Vector2(0, itemHeight)))
                    {
                        _selectedPackageGuid = meta.Guid;
                    }
                    
                    float textHeight = ImGui.GetTextLineHeight();
                    float verticalCenteringOffset = (itemHeight - textHeight) * 0.5f;
                    
                    ImGui.SameLine();
                    ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 6.0f);
                    ImGui.SetCursorPosY(ImGui.GetCursorPosY() + verticalCenteringOffset);
                    
                    ImGui.Text(meta.Name);
                    
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
        
        ImGui.SetWindowFontScale(2f);
        ImGui.Text(meta.Name);
        ImGui.SetWindowFontScale(1.0f);
        
        ImGui.TextDisabled($"Author  : {meta.Author}");
        ImGui.TextDisabled($"Company : {meta.Company}");
        ImGui.TextDisabled($"Version : {meta.Version}");
        ImGuiEx.SelectableTextDisabled($"( {meta.Guid} )");
        ImGui.Separator();
        
        ImGui.Spacing();
        ImGui.TextWrapped(meta.Description);
    }
}

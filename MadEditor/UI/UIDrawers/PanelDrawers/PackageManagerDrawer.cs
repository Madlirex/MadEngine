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
        // Unique string formatting logic for clean layout table isolation
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
            foreach (var pair in PackageManager.PackagesMetas)
            {
                PackageMeta meta = pair.Value;
                
                bool isSelected = _selectedPackageGuid == meta.Guid;
                ImGui.PushID(meta.Guid.ToString());

                // FIX 2: Calculate the exact available width explicitly 
                // Instead of passing a broken negative number, we measure the region and subtract space for the button.
                float availableWidth = ImGui.GetContentRegionAvail().X;
                float selectableWidth = meta.IsRemovable ? availableWidth - 60f : availableWidth;

                if (ImGui.Selectable(meta.Name, isSelected, ImGuiSelectableFlags.AllowOverlap,
                        new Vector2(selectableWidth, 0)))
                {
                    _selectedPackageGuid = meta.Guid;
                }

                if (meta.IsRemovable)
                {
                    ImGui.SameLine();
                    // FIX 3: Cleaned alignment calculations relative to the current cell boundaries
                    ImGui.SetCursorPosX(ImGui.GetCursorPosX() + ImGui.GetContentRegionAvail().X - 55);

                    if (ImGui.Button("Remove", new Vector2(50, 0)))
                    {
                        Console.WriteLine($"Removing {meta.Name}");
                    }
                }
                
                ImGui.PopID();
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

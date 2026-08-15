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

            if(ImGui.TableNextColumn())
                DrawLeftPanel();
            
            if(ImGui.TableNextColumn())
                DrawRightPanel();
            
            ImGui.EndTable();
        }
    }

    private void DrawLeftPanel()
    {
        ImGui.TextDisabled("Packages");
        ImGui.Separator();

        bool openChild = ImGui.BeginChild("PackageListRegion", new Vector2(0, 0), ImGuiChildFlags.None);
        
        if (!openChild || !ImGui.BeginTable("PackageListTable", 2, ImGuiTableFlags.NoBordersInBody))
        {
            ImGui.EndChild();
            return;
        }

        float squareSize = ImGui.GetFrameHeight();
        
        ImGui.TableSetupColumn("NameColumn", ImGuiTableColumnFlags.WidthStretch);
        ImGui.TableSetupColumn("ActionColumn", ImGuiTableColumnFlags.WidthFixed, squareSize + 3f);
        
        foreach (var pair in PackageManager.PackagesMetas)
        {
            PackageMeta meta = pair.Value;
            bool isSelected = _selectedPackageGuid == meta.Guid;

            ImGui.PushID(meta.Guid.ToString());
            
            ImGui.TableNextRow(ImGuiTableRowFlags.None, squareSize);

            ImGui.TableNextColumn();
            
            if (ImGui.Selectable($"##Selectable_{meta.Guid}", isSelected,
                    ImGuiSelectableFlags.SpanAllColumns | ImGuiSelectableFlags.AllowOverlap,
                    new Vector2(0, squareSize)))
            {
                _selectedPackageGuid = meta.Guid;
            }

            float textHeight = ImGui.GetTextLineHeight();
            float verticalCenteringOffset = (squareSize - textHeight) * 0.5f;

            ImGui.SameLine();
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 6.0f);
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + verticalCenteringOffset);

            ImGui.Text(meta.Name);
                
            ImGui.TableNextColumn();
            
            if (meta.IsRemovable)
            {
                ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(3f, 0f));
                
                if (ImGui.Button("^", new Vector2(squareSize, squareSize)))
                {
                    Console.WriteLine($"Updating {meta.Name}");
                }

                ImGui.PopStyleVar();
            }

            ImGui.PopID();
        }

        ImGui.EndTable();
        ImGui.EndChild();
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

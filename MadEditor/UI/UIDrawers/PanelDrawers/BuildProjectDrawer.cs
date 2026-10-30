using ImGuiNET;
using MadEngine.Core;
using NativeFileDialogSharp;
using System.Numerics;
using MadEditor.Menubar;

namespace MadEditor;

[Order(2600)]
public class OpenProjectSettingsCommand : OpenPanelCommand<BuildProjectDrawer>
{
    public override string Path { get; set; } = "File/Build";
}

[CustomName("Build Project")]
public class BuildProjectDrawer : PanelDrawer
{
    public override PanelRegion PanelRegion { get; set; } = PanelRegion.Floating;

    private string _gameName = "MyMadEngineGame";
    private string _exportPath = "";
    private string _statusMessage = "";
    private Vector4 _statusColor = new Vector4(1, 1, 1, 1);

    public override void Draw(EditorUIContext context)
    {
        ImGui.TextDisabled("Configure Standalone Game Build Pipeline");
        ImGui.Separator();
        ImGui.Spacing();
        
        ImGuiEx.InputTextDynamic("Game Name", ref _gameName, 64);
        ImGui.Spacing();
        
        ImGuiEx.InputTextDynamic("Export Path", ref _exportPath, 256, ImGuiInputTextFlags.ReadOnly);
        ImGui.SameLine();
        
        if (ImGui.Button("Browse..."))
        {
            string selectedPath = Dialog.FolderPicker().Path;
            if (!string.IsNullOrWhiteSpace(selectedPath))
            {
                _exportPath = selectedPath;
            }
        }

        ImGui.Spacing();
        ImGui.Separator();
        ImGui.Spacing();
        
        bool isValid = !string.IsNullOrWhiteSpace(_gameName) && !string.IsNullOrWhiteSpace(_exportPath);
        if (!isValid)
        {
            ImGui.BeginDisabled();
        }

        if (ImGui.Button("Export & Build Standalone Game", new Vector2(-1, 40)))
        {
            RunExportPipeline();
        }

        if (!isValid)
        {
            ImGui.EndDisabled();
        }

        if (string.IsNullOrEmpty(_statusMessage)) return;
        ImGui.Spacing();
        ImGui.TextColored(_statusColor, _statusMessage);
    }
    
    private void RunExportPipeline()
    {
        _statusMessage = "Compiling and packaging project assets...";
        _statusColor = new Vector4(1, 0.8f, 0, 1);

        try
        {
            string[] sourceFiles = ProjectManager.GetAllFiles("*.cs");

            var (runtimeBytes, _) = ScriptCompiler.CompileProject(sourceFiles);

            if (runtimeBytes == null)
            {
                _statusMessage = "Compilation Error: Failed to compile GameScripts assembly.";
                _statusColor = new Vector4(1, 0.2f, 0.2f, 1);
                return;
            }
            
            BuildManager.ExportGame(
                _exportPath, 
                _gameName, 
                runtimeBytes
            );

            _statusMessage = $"Success! Game successfully exported to: {Path.Combine(_exportPath, _gameName + ".exe")}";
            _statusColor = new Vector4(0.2f, 1, 0.2f, 1);
        }
        catch (Exception ex)
        {
            _statusMessage = $"Build Failure: {ex.Message}";
            _statusColor = new Vector4(1, 0.2f, 0.2f, 1);
            Console.WriteLine(ex.ToString());
        }
    }
}

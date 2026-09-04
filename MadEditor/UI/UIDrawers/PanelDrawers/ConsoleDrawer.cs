using System.Numerics;
using ImGuiNET;
using MadEngine.Core;

namespace MadEditor;

[CustomName("Console")]
public class ConsoleDrawer : PanelDrawer
{
    public override PanelRegion PanelRegion { get; set; } = PanelRegion.Bottom;
    
    private bool _showInfo = true;
    private bool _showWarnings = true;
    private bool _showErrors = true;

    public override void Draw(EditorUIContext context)
    {
        if (ImGui.Button("Clear")) Debug.Clear();
        ImGui.SameLine();
    
        ImGui.Checkbox("Info", ref _showInfo); ImGui.SameLine();
        ImGui.Checkbox("Warning", ref _showWarnings); ImGui.SameLine();
        ImGui.Checkbox("Error", ref _showErrors);

        ImGui.Separator();
        
        ImGui.BeginChild("LogScrollArea");
    
        var masterLogs = Debug.GetReadOnlyLogs();
        foreach (var log in masterLogs)
        {
            switch (log.Type)
            {
                case LogType.Info when !_showInfo:
                case LogType.Warning when !_showWarnings:
                case LogType.Error when !_showErrors:
                    continue;
            }

            Vector4 textColor = log.Type switch
            {
                LogType.Warning => new Vector4(1f, 0.8f, 0.2f, 1f),
                LogType.Error => new Vector4(1f, 0.3f, 0.3f, 1f),
                _ => new Vector4(0.9f, 0.9f, 0.9f, 1f)
            };

            ImGui.PushStyleColor(ImGuiCol.Text, textColor);

            ImGui.Text(log.Count > 1
                ? $"[{log.Timestamp:HH:mm:ss}] ({log.Count}) {log.Message}"
                : $"[{log.Timestamp:HH:mm:ss}] {log.Message}");

            ImGui.PopStyleColor();
        }
    
        ImGui.EndChild();
    }
}
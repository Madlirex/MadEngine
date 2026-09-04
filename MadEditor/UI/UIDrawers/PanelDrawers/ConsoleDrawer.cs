using System;
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
    private bool _collapse = true;

    private LogEntry? _selectedLog;
    
    private float _bottomPaneHeight = 140.0f; 
    private const float MinPaneHeight = 40.0f;

    public override void Draw(EditorUIContext context)
    {
        if (ImGui.Button("Clear"))
        {
            Debug.Clear();
            _selectedLog = null;
        }
        ImGui.SameLine();
        
        ImGui.Checkbox("Collapse", ref _collapse);
        
        int infoCount = 0, warnCount = 0, errorCount = 0;
        var masterLogs = Debug.GetReadOnlyLogs();
        
        foreach (var log in masterLogs)
        {
            switch (log.Type)
            {
                case LogType.Info: infoCount += _collapse ? log.Count : 1; break;
                case LogType.Warning: warnCount += _collapse ? log.Count : 1; break;
                case LogType.Error: errorCount += _collapse ? log.Count : 1; break;
            }
        }
        
        float scrollbarWidth = ImGui.GetStyle().ScrollbarSize;
        float rightSideOffset = ImGui.GetWindowWidth() - (320.0f + scrollbarWidth); // Bumped padding slightly for clean text spacing
        
        if (ImGui.GetCursorPosX() < rightSideOffset) 
        {
            ImGui.SameLine(rightSideOffset);
        }
        else
        {
            ImGui.SameLine();
        }
        
        // FIX: Pass raw static string literals to the checkboxes so ImGui never resets their ID hashes.
        // Then append the dynamic numbers via a SameLine text command.
        ImGui.Checkbox("##info_filter_cb", ref _showInfo); ImGui.SameLine();
        ImGui.Text($"Info ({infoCount})"); ImGui.SameLine();
        
        ImGui.Checkbox("##warn_filter_cb", ref _showWarnings); ImGui.SameLine();
        ImGui.Text($"Warn ({warnCount})"); ImGui.SameLine();
        
        ImGui.Checkbox("##error_filter_cb", ref _showErrors); ImGui.SameLine();
        ImGui.Text($"Error ({errorCount})");

        ImGui.Separator();
        
        float activeBottomHeight = _selectedLog != null ? _bottomPaneHeight : 0.0f;
        
        float totalAvailableHeight = ImGui.GetContentRegionAvail().Y;
        float topPaneHeight = totalAvailableHeight - activeBottomHeight - ImGui.GetStyle().ItemSpacing.Y;
        
        if (topPaneHeight < 40.0f && _selectedLog != null)
        {
            topPaneHeight = 40.0f;
            _bottomPaneHeight = totalAvailableHeight - topPaneHeight - ImGui.GetStyle().ItemSpacing.Y;
        }
        
        ImGui.BeginChild("LogScrollArea", new Vector2(0, topPaneHeight), ImGuiChildFlags.None);
        
        int logIndex = 0;
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
            string displayString = $"[{log.Timestamp:HH:mm:ss}] {log.Message}";
            bool isSelected = _selectedLog == log;
            
            ImGui.PushID(logIndex++);
            if (ImGui.Selectable("##log_row", isSelected, ImGuiSelectableFlags.AllowDoubleClick, new Vector2(0, 20)))
            {
                _selectedLog = log;
            }
            ImGui.PopID();

            ImGui.SameLine(5.0f);
            ImGui.Text(displayString);
            
            if (_collapse && log.Count > 1)
            {
                string countText = log.Count.ToString();
                float textWidth = ImGui.CalcTextSize(countText).X;
                float targetXPosition = ImGui.GetWindowWidth() - textWidth - (scrollbarWidth + 25.0f);
                
                ImGui.SameLine(targetXPosition);
                ImGui.TextDisabled(countText);
            }

            ImGui.PopStyleColor();
        }
        
        ImGui.EndChild();

        if (_selectedLog == null) return;
        
        ImGui.SetNextItemAllowOverlap();
        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0f, 0f, 0f, 0f));
        ImGui.PushStyleColor(ImGuiCol.ButtonActive, new Vector4(0.3f, 0.5f, 0.8f, 0.4f)); 
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.2f, 0.2f, 0.2f, 0.6f));
        
        ImGui.Button("##console_splitter", new Vector2(-1, 4.0f));
        
        if (ImGui.IsItemHovered()) ImGui.SetMouseCursor(ImGuiMouseCursor.ResizeNS);

        if (ImGui.IsItemActive())
        {
            float deltaY = ImGui.GetIO().MouseDelta.Y;
            _bottomPaneHeight -= deltaY;
            
            _bottomPaneHeight = Math.Clamp(_bottomPaneHeight, MinPaneHeight, totalAvailableHeight - 60.0f);
        }
        ImGui.PopStyleColor(3);
        
        ImGui.BeginChild("StackTraceArea", new Vector2(0, _bottomPaneHeight), ImGuiChildFlags.None, ImGuiWindowFlags.HorizontalScrollbar);
            
        ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1f, 1f, 1f, 1f));
        ImGui.TextWrapped($"Selected: {_selectedLog.Message}");
        ImGui.PopStyleColor();
            
        ImGui.Separator();
        ImGuiEx.SelectableTextMultiline(_selectedLog.StackTrace, ImGui.GetStyle().Colors[(int)ImGuiCol.Text]);
            
        ImGui.EndChild();
    }
}

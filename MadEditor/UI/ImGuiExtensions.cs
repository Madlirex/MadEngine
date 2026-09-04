using System.Numerics;
using ImGuiNET;

namespace MadEditor;

public static class ImGuiEx
{
    public static void SelectableText(string text)
    {
        ImGui.PushStyleColor(ImGuiCol.FrameBg, Vector4.Zero);
        ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, Vector4.Zero);
        ImGui.PushStyleColor(ImGuiCol.Border, Vector4.Zero);
        
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, Vector2.Zero);
        
        float textWidth = ImGui.CalcTextSize(text).X;
        
        uint bufferSize = (uint)Math.Max(text.Length * 2, 256);
        ImGui.InputText($"##SelectableTextLiteral", ref text, bufferSize, ImGuiInputTextFlags.ReadOnly);

        ImGui.PopStyleVar();
        ImGui.PopStyleColor(3);
    }
    
    public static void SelectableTextMultiline(string text, Vector4 textColor, Vector2 size = default)
    {
        if (size == default) size = new Vector2(-1, -1);
        
        ImGui.PushStyleColor(ImGuiCol.FrameBg, Vector4.Zero);
        ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, Vector4.Zero);
        ImGui.PushStyleColor(ImGuiCol.FrameBgActive, Vector4.Zero);
        ImGui.PushStyleColor(ImGuiCol.Border, Vector4.Zero);
        ImGui.PushStyleColor(ImGuiCol.Text, textColor);
        
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, Vector2.Zero);
        
        uint bufferSize = (uint)Math.Max(text.Length * 2, 4096);
        
        ImGui.InputTextMultiline("##SelectableTextMultilineLiteral", ref text, bufferSize, size, ImGuiInputTextFlags.ReadOnly);

        ImGui.PopStyleVar();
        ImGui.PopStyleColor(5);
    }

    
    public static void SelectableTextDisabled(string text)
    {
        Vector4 disabledTextColor = ImGui.GetStyle().Colors[(int)ImGuiCol.TextDisabled];
        
        ImGui.PushStyleColor(ImGuiCol.Text, disabledTextColor);
        
        SelectableText(text);

        ImGui.PopStyleColor();
    }
    
    public static void SelectableTextUnformatted(string text)
    {
        Vector4 standardTextColor = ImGui.GetStyle().Colors[(int)ImGuiCol.Text];
        
        ImGui.PushStyleColor(ImGuiCol.Text, standardTextColor);
        
        SelectableText(text);

        ImGui.PopStyleColor();
    }

    public static bool IsClicked(ImGuiMouseButton button)
    {
        if (!ImGui.IsItemHovered() || !ImGui.IsMouseReleased(button)) return false;
        return !ImGui.IsMouseDragging(button);
    }

    public static bool IsClickedOutside(ImGuiMouseButton button)
    {
        return ImGui.IsWindowHovered(ImGuiHoveredFlags.ChildWindows) && ImGui.IsMouseClicked(button) && !ImGui.IsAnyItemHovered();
    }
}
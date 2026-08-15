using System.Numerics;
using ImGuiNET;

namespace MadEditor;

public static class ImGuiEx
{
    public static void SelectableText(string text)
    {
        ImGui.PushStyleColor(ImGuiCol.Border, Vector4.Zero);
        
        float textWidth = ImGui.CalcTextSize(text).X + ImGui.GetStyle().FramePadding.X * 2.0f;
        ImGui.SetNextItemWidth(textWidth);
        
        ImGui.InputText($"##SelText_{text.GetHashCode()}", ref text, (uint)text.Length + 1, ImGuiInputTextFlags.ReadOnly);

        ImGui.PopStyleColor(2);
    }
    
    public static void SelectableTextDisabled(string text)
    {
        Vector4 disabledTextColor = ImGui.GetStyle().Colors[(int)ImGuiCol.TextDisabled];
        
        ImGui.PushStyleColor(ImGuiCol.Text, disabledTextColor);
        
        SelectableText(text);

        ImGui.PopStyleColor();
    }
}
using System;
using ImGuiNET;

namespace MadEditor;

public static class ShortcutInputEngine
{
    public static bool IsShortcutPressed(ImGuiKey[] keys)
    {
        if (keys == null || keys.Length == 0) return false;
        
        bool allKeysDown = Array.TrueForAll(keys, ImGui.IsKeyDown);
        
        return allKeysDown && keys.Any(ImGui.IsKeyPressed);
    }
}
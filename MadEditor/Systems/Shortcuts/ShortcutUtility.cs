using System.Text;
using ImGuiNET;

namespace MadEditor;

public static class ShortcutUtility
{
    public static string ToDisplayString(ImGuiKey[] keys)
    {
        if (keys == null || keys.Length == 0) return string.Empty;

        var sb = new StringBuilder();
        for (int i = 0; i < keys.Length; i++)
        {
            if (i > 0) sb.Append('+');

            string name = keys[i].ToString();
            
            if (name.StartsWith("Mod")) name = name[3..];
            if (name.StartsWith("Key_")) name = name[4..];

            sb.Append(name);
        }
        return sb.ToString();
    }
}
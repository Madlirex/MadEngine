using ImGuiNET;
using MadEngine.Core;

namespace MadEditor;

public class GeneralProjectSettings : ProjectSettingsTab
{
    public override string Path => "General";
    public override void Draw(EditorUIContext context)
    {
        
    }
}

public class PerformanceProjectSettings : ProjectSettingsTab
{
    public override string Path => "Other/Performance";
    public override void Draw(EditorUIContext context)
    {
        ImGui.Text("Hello World!");
    }
}
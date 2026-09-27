using ImGuiNET;
using MadEngine.Core;

namespace MadEditor;

public class GeneralProjectSettings : ProjectSettingsTab<ProjectInfo>
{
    public override string Path => "General";
    public override void Draw(EditorUIContext context)
    {
        string nameBuffer = Settings.Name;
        if (ImGuiEx.InputTextDynamic("Project Name", ref nameBuffer))
        {
            Settings.Name = nameBuffer;
        }
        
        ImGui.Text("Description");
        string descBuffer = Settings.Description;
        if (ImGuiEx.InputTextMultilineDynamic("Description", ref descBuffer, new System.Numerics.Vector2(-1, 60)))
        {
            Settings.Description = descBuffer;
        }
        ImGui.Spacing();
        
        int major = Settings.Version.Major;
        int minor = Settings.Version.Minor;
        int build = Settings.Version.Build < 0 ? 0 : Settings.Version.Build;
        
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new System.Numerics.Vector2(2.0f, ImGui.GetStyle().ItemSpacing.Y));

        ImGui.SetNextItemWidth(30);
        bool majorChanged = ImGui.InputInt("##Major", ref major, 0, 0); 
        ImGui.SameLine();

        ImGui.Text("."); 
        ImGui.SameLine();

        ImGui.SetNextItemWidth(30);
        bool minorChanged = ImGui.InputInt("##Minor", ref minor, 0, 0); 
        ImGui.SameLine();

        ImGui.Text("."); 
        ImGui.SameLine();

        ImGui.SetNextItemWidth(30);
        bool buildChanged = ImGui.InputInt("##Build", ref build, 0, 0); 
        ImGui.SameLine();
        
        ImGui.PopStyleVar();
        
        ImGui.SameLine(0.0f, 8.0f);
        ImGui.Text("Version (Major.Minor.Build)");

        if (majorChanged || minorChanged || buildChanged)
        {
            Settings.Version = new Version(Math.Max(0, major), Math.Max(0, minor), Math.Max(0, build));
        }
        ImGui.Separator();
        ImGui.Spacing();
        
        string authorBuffer = Settings.Author;
        if (ImGuiEx.InputTextDynamic("Author", ref authorBuffer))
        {
            Settings.Author = authorBuffer;
        }
        
        string companyBuffer = Settings.Company;
        if (ImGuiEx.InputTextDynamic("Company", ref companyBuffer))
        {
            Settings.Company = companyBuffer;
        }
    }
}

public class PerformanceProjectSettingsTab : ProjectSettingsTab<PerformanceSettings>
{
    public override string Path => "Other/Performance";
    public override void Draw(EditorUIContext context)
    {
        int maxFps = Settings.MaxFps;

        if (ImGui.DragInt("Max FPS", ref maxFps))
        {
            Settings.MaxFps = maxFps;
        }
    }
}
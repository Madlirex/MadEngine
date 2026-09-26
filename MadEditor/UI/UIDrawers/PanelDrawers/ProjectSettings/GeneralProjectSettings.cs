using ImGuiNET;

namespace MadEditor;

public class GeneralProjectSettings : ProjectSettingsTab
{
    public override string Path => "General";
    public override void Draw(EditorUIContext context)
    {
        ProjectInfo project = ProjectManager.ProjectInfo;

        string nameBuffer = project.Name;
        if (ImGui.InputText("Project Name", ref nameBuffer, 128))
        {
            project.Name = nameBuffer;
        }
        
        string descBuffer = project.Description;
        if (ImGui.InputTextMultiline("Description", ref descBuffer, 512, new System.Numerics.Vector2(-1, 60)))
        {
            project.Description = descBuffer;
        }
        ImGui.Spacing();
        
        ImGui.Text("Version Matrix");
        int major = project.Version.Major;
        int minor = project.Version.Minor;
        int build = project.Version.Build < 0 ? 0 : project.Version.Build;
        
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
            project.Version = new Version(Math.Max(0, major), Math.Max(0, minor), Math.Max(0, build));
        }
        ImGui.Separator();
        ImGui.Spacing();
        
        string authorBuffer = project.Author;
        if (ImGui.InputText("Author", ref authorBuffer, 128))
        {
            project.Author = authorBuffer;
        }
        
        string companyBuffer = project.Company;
        if (ImGui.InputText("Company", ref companyBuffer, 128))
        {
            project.Company = companyBuffer;
        }
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
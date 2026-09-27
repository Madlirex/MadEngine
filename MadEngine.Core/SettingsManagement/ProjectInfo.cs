namespace MadEngine.Core;

public class ProjectInfo : ProjectSettings
{
    public string Name { get; set; } = "New Project";
    public string Description { get; set; } = "Cool new game!";
    public Version Version { get; set; } = new(1, 0, 0);
    public Version EditorVersion { get; set; } = new(1, 0, 0);
    public DateTime LastOpened { get; set; } = DateTime.Now;
    
    public void RefreshLastOpened() => LastOpened = DateTime.Now;
    
    public string Author { get; set; } = "DefaultAuthor";
    public string Company { get; set; } = "DefaultCompany";
    public override string GetFileName() => "project";
    public override string GetFileExtension() => "madx";
    public override string GetFolderPath() => Application.Directory;
}
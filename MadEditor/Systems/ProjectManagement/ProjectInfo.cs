namespace MadEditor;

public class ProjectInfo
{
    public string Name { get; set; } = "New Project";
    public string Description { get; set; } = "Cool new game!";
    public Version Version { get; set; } = new Version(1, 0, 0);
    
    public string Author { get; set; } = "DefaultAuthor";
    public string Company { get; set; } = "DefaultCompany";
}
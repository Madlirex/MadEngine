namespace MadEditor.PackageManagement;

public record PackageMeta
{
    public required string Name { get; init; }
    public required string Author { get; init; }
    public required string Company { get; init; }
    public required string Description { get; init; }
    public required Version Version { get; init; }
    
    public bool IsDevelopment { get; init; }
    public bool IsEnabled { get; init; } = true;
    public bool IsModifiable { get; init; } = true;
    public bool IsRemovable { get; init; } = true;
}
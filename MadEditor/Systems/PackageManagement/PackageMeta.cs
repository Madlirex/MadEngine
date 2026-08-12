namespace MadEditor.PackageManagement;

public record PackageMeta (
    string Name,
    string Description,
    string Author,
    string Company,
    Version Version,
    bool IsDevelopment = false,
    bool IsEnabled = true,
    bool IsModifiable = true,
    bool IsRemovable = true
);
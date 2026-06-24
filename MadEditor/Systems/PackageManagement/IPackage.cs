namespace MadEditor.PackageManagement;

public interface IPackage
{
    public string Name { get; }
    public int Version { get; }
}
namespace MadEngine.Core;

public abstract class ProjectSettings
{
    public abstract string GetFileName();
    public virtual string GetFileExtension() => "json";
    public virtual string GetFolderPath() => Application.ConfigPath;
}
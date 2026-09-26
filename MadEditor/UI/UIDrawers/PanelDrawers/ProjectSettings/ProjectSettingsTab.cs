namespace MadEditor;

public abstract class ProjectSettingsTab
{
    public Guid Guid { get; } = Guid.NewGuid();
    public abstract string Path { get; }

    public abstract void Draw(EditorUIContext context);
}
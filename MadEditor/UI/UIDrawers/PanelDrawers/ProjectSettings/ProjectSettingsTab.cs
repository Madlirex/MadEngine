namespace MadEditor;

public abstract class ProjectSettingsTab
{
    public abstract string Path { get; }

    public abstract void Draw(EditorUIContext context);
}
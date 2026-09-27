using MadEngine.Core;

namespace MadEditor;

public interface IProjectSettingsTab
{
    ProjectSettings Settings { get; }
    
    public Guid Guid { get; }
    public string Path { get; }

    public void Draw(EditorUIContext context);
}

public abstract class ProjectSettingsTab<T> : IProjectSettingsTab where T : ProjectSettings, new()
{
    public T Settings => ProjectSettingsRegistry.GetSettings<T>() ?? ProjectSettingsRegistry.CreateSettings<T>();
    ProjectSettings IProjectSettingsTab.Settings => Settings; 
    public Guid Guid { get; } = Guid.NewGuid();
    public abstract string Path { get; }
    public abstract void Draw(EditorUIContext context);
}
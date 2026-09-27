namespace MadEngine.Core;

public static class ProjectSettingsRegistry
{
    private static ProjectSettingsEngine Instance => RegistryBootstrapper.Get<ProjectSettingsEngine>();

    public static IReadOnlyList<ProjectSettings> ProjectSettingsList => Instance.ProjectSettings;
    
    public static T? GetSettings<T>() where T : ProjectSettings => Instance.GetSettings<T>();
    public static T CreateSettings<T>() where T : ProjectSettings, new() => Instance.CreateSettings<T>();
}

internal class ProjectSettingsEngine : Registry
{
    public IReadOnlyList<ProjectSettings> ProjectSettings => _projectSettings;
    private List<ProjectSettings> _projectSettings = [];
    
    public override void Initialize()
    {
        Discover();
    }

    private void Discover()
    {
        _projectSettings.Clear();
        
        var types = AssemblyProvider.GetTypesImplementing(typeof(ProjectSettings));
        foreach (var type in types)
        {
            if (Activator.CreateInstance(type) is ProjectSettings settings)
                _projectSettings.Add(settings);
        }
    }

    public T? GetSettings<T>() where T : ProjectSettings
    {
        return (T?)_projectSettings.Find(x => x.GetType() == typeof(T));
    }

    public T CreateSettings<T>() where T : ProjectSettings, new()
    {
        T settings = new T();
        _projectSettings.Add(settings);
        return settings;
    }
}
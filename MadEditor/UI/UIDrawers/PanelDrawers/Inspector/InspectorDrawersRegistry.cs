namespace MadEditor;

public static class InspectorDrawersRegistry
{
    private static readonly InspectorDrawersEngine Instance = RegistryBootstrapper.Get<InspectorDrawersEngine>();
    
    public static IInspectorDrawer GetDrawer(Type type) => Instance.GetDrawerInternal(type) ?? throw new ArgumentException($"Inspector for type {type} does not exist");
}

internal class InspectorDrawersEngine : Registry
{
    private readonly Dictionary<Type, IInspectorDrawer> _inspectorDrawers = [];
    
    public override void Initialize()
    {
        _inspectorDrawers.Clear();
        
        var types = FindTypesImplementing<IInspectorDrawer>();
        foreach (var type in types)
        {
            InitializeType(type, _inspectorDrawers);
        }
    }

    internal IInspectorDrawer? GetDrawerInternal(Type type) => _inspectorDrawers.GetValueOrDefault(type);
}
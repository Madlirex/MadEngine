using ImGuiNET;

namespace MadEditor;

public static class InspectorDrawersRegistry
{
    private static readonly InspectorDrawersEngine Instance = RegistryBootstrapper.Get<InspectorDrawersEngine>();
    
    public static IInspectorDrawer GetDrawer(Type? type) => Instance.GetDrawerInternal(type) ?? Instance.DefaultDrawer;
}

internal class InspectorDrawersEngine : Registry
{
    private readonly Dictionary<Type, IInspectorDrawer> _inspectorDrawers = [];
    public DefaultDrawer DefaultDrawer = new();
    
    public override void Initialize()
    {
        _inspectorDrawers.Clear();
        
        var types = FindTypesImplementing<IInspectorDrawer>();
        foreach (var type in types)
        {
            if (Activator.CreateInstance(type) is not IInspectorDrawer drawer) continue;
            _inspectorDrawers.TryAdd(drawer.Type, drawer);
        }
    }

    internal IInspectorDrawer? GetDrawerInternal(Type? type) => type == null ? null : _inspectorDrawers.GetValueOrDefault(type);
}

public class DefaultDrawer : IInspectorDrawer
{
    public Type Type => typeof(object);
    public void Draw(EditorUIContext context)
    {
        ImGui.TextDisabled("Select something");
    }
}
using ImGuiNET;

namespace MadEditor;

public static class InspectorDrawersRegistry
{
    private static readonly InspectorDrawersEngine Instance = RegistryBootstrapper.Get<InspectorDrawersEngine>();
    
    public static IInspectorDrawer GetDrawer(Type? type) => Instance.GetDrawerInternal(type);
}

internal class InspectorDrawersEngine : Registry
{
    private readonly Dictionary<Type, IInspectorDrawer> _inspectorDrawers = [];
    public NoneDrawer NoneDrawer = new();
    
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

    internal IInspectorDrawer GetDrawerInternal(Type? type)
    {
        if (type == null) return NoneDrawer;
        
        if (_inspectorDrawers.TryGetValue(type, out var exactDrawer))
        {
            return exactDrawer;
        }
        
        Type? currentType = type.BaseType;
        while (currentType != null)
        {
            if (_inspectorDrawers.TryGetValue(currentType, out var baseDrawer))
            {
                _inspectorDrawers.TryAdd(type, baseDrawer);
                return baseDrawer;
            }
            currentType = currentType.BaseType;
        }
        
        foreach (var interfaceType in type.GetInterfaces())
        {
            if (!_inspectorDrawers.TryGetValue(interfaceType, out var interfaceDrawer)) continue;
            _inspectorDrawers.TryAdd(type, interfaceDrawer);
            return interfaceDrawer;
        }

        return NoneDrawer;
    }
}

public class NoneDrawer : IInspectorDrawer
{
    public Type Type => typeof(object);
    public void Draw(EditorUIContext context)
    {
        ImGui.TextDisabled("Select something");
    }
}
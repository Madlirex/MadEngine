using System.Reflection;
using MadEngine.Core;

namespace MadEditor;

public static class ProjectSettingsTabsRegistry
{
    private static ProjectSettingsTabsEngine Instance => RegistryBootstrapper.Get<ProjectSettingsTabsEngine>();
    
    public static IReadOnlyList<IProjectSettingsTab> Tabs => Instance.Tabs;
    public static Dictionary<string, int> LoadCategoryWeights() => Instance.LoadCategoryWeights();
}

internal class ProjectSettingsTabsEngine : Registry
{
    internal List<IProjectSettingsTab> Tabs = [];
    
    public override void Initialize()
    {
        Discover();
    }
    
    private void Discover()
    {
        var types = AssemblyProvider.GetTypesImplementing(typeof(IProjectSettingsTab));

        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type);
            if (instance is IProjectSettingsTab tab)
            {
                Tabs.Add(tab);
            }
        }
    }

    private int GetOrder(IProjectSettingsTab tab)
    {
        OrderAttribute? attr = tab.GetType().GetCustomAttribute<OrderAttribute>();
        return attr?.Order ?? 0;
    }
    
    internal Dictionary<string, int> LoadCategoryWeights()
    {
        Dictionary<string, int> categoryWeights = new Dictionary<string, int>();

        var attributes = AssemblyProvider.Assemblies.SelectMany(x => x.GetCustomAttributes<CategoryOrderAttribute<IProjectSettingsTab>>());

        foreach (var attribute in attributes)
        {
            categoryWeights[attribute.Name] = attribute.Order;
        }

        return categoryWeights;
    }
}
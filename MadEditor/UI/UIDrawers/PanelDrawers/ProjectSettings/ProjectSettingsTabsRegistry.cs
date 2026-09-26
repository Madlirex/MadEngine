using System.Reflection;
using MadEngine.Core;

namespace MadEditor;

public static class ProjectSettingsTabsRegistry
{
    private static ProjectSettingsTabsEngine Instance => RegistryBootstrapper.Get<ProjectSettingsTabsEngine>();
    
    public static IReadOnlyList<ProjectSettingsTab> Tabs => Instance.Tabs;
}

internal class ProjectSettingsTabsEngine : Registry
{
    internal List<ProjectSettingsTab> Tabs = [];
    
    public override void Initialize()
    {
        Discover();
    }
    
    private void Discover()
    {
        var types = ScriptDomain.GetTypesImplementing(typeof(ProjectSettingsTab));

        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type);
            if (instance is ProjectSettingsTab tab)
            {
                Tabs.Add(tab);
            }
        }
        
        Tabs.Sort((x, y) => GetOrder(x).CompareTo(GetOrder(y)));
    }

    private int GetOrder(ProjectSettingsTab tab)
    {
        OrderAttribute? attr = tab.GetType().GetCustomAttribute<OrderAttribute>();
        return attr?.Order ?? 0;
    }
}
using System.Reflection;
using ImGuiNET;
using MadEngine.Core;

namespace MadEditor;

public static class MenubarCommandsRegistry
{
    private static readonly MenubarCommandsEngine Instance = RegistryBootstrapper.Get<MenubarCommandsEngine>();
    
    public static void DiscoverCommands() => Instance.DiscoverCommands();
    public static void RegisterCommand(MenubarCommand command) => Instance.RegisterCommand(command);
    public static void CreateCommand(Type type) => Instance.CreateCommand(type);
    
    public static void Draw(EditorUIContext context) => Instance.Draw(context);
}

internal class MenubarCommandsEngine : Registry
{
    private readonly Dictionary<string, int> _categoryWeights = new(StringComparer.Ordinal);
    internal readonly List<MenubarCommand> MenubarCommands = [];
    
    private MenuTreeRenderer? _menuTreeRenderer;
    
    public override void Initialize()
    {
        LoadCategoryWeights();
        _menuTreeRenderer = new MenuTreeRenderer(_categoryWeights);
        DiscoverCommands();
    }

    public void LoadCategoryWeights()
    {
        _categoryWeights.Clear();

        var attributes = ScriptDomain.Assemblies.SelectMany(x => x.GetCustomAttributes<CategoryOrderAttribute>());

        foreach (var attribute in attributes)
        {
            _categoryWeights[attribute.Name] = attribute.Order;
        }
    }

    public void DiscoverCommands()
    {
        MenubarCommands.Clear();
        
        var types = ScriptDomain.GetTypesImplementing(typeof(MenubarCommand));
        foreach (var type in types)
        {
            if (Activator.CreateInstance(type) is not MenubarCommand command) continue;
            MenubarCommands.Add(command);
        }

        RebuildTree();
    }
    
    private void RebuildTree()
    {
        var menuData = MenubarCommands.Select(cmd => {
            var orderAttr = cmd.GetType().GetCustomAttribute<OrderAttribute>();
            return new CommandMenuData(cmd, cmd.Path, orderAttr?.Order ?? 0, cmd.Shortcut);
        });
        
        _menuTreeRenderer?.RegenerateTree(menuData);
    }
    
    public MenubarCommand? CreateCommand(Type type)
    {
        return Activator.CreateInstance(type) as MenubarCommand;
    }
    
    public void RegisterCommand(MenubarCommand command)
    {
        if (MenubarCommands.Contains(command)) return;
        
        MenubarCommands.Add(command);
        
        RebuildTree(); 
    }


    public void Draw(EditorUIContext context)
    {
        if (!ImGui.BeginMainMenuBar()) return;
        
        _menuTreeRenderer?.Draw(cmd => context.EnqueueCommand((MenubarCommand)cmd));
        
        ImGui.EndMainMenuBar();
    }
}
using System.Reflection;
using ImGuiNET;
using MadEngine.Core;

namespace MadEditor;

public static class PopupCommandsRegistry
{
    private static readonly PopupCommandsEngine Instance = RegistryBootstrapper.Get<PopupCommandsEngine>();
    
    public static void DiscoverCommands() => Instance.DiscoverCommands();
    public static void RegisterCommand(IPopupCommand command) => Instance.RegisterCommand(command);
    public static void CreateCommand(Type type) => Instance.CreateCommand(type);
    
    public static void RenderContextMenu(object? target) => Instance.RenderContextMenu(target);
}

internal class PopupCommandsEngine : Registry
{
    private readonly Dictionary<string, int> _categoryWeights = new(StringComparer.Ordinal);
    internal readonly List<IPopupCommand> PopupCommands = [];

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
        PopupCommands.Clear();
        var types = ScriptDomain.GetTypesImplementing(typeof(IPopupCommand));
        foreach (var type in types)
        {
            if (Activator.CreateInstance(type) is not IPopupCommand command) continue;
            PopupCommands.Add(command);
        }
    }

    public void RegisterCommand(IPopupCommand command)
    {
        PopupCommands.Add(command);
        
        RebuildTree();
    }
    
    private void RebuildTree()
    {
        var menuData = PopupCommands.Select(cmd => {
            var orderAttr = cmd.GetType().GetCustomAttribute<OrderAttribute>();
            return new CommandMenuData(cmd, cmd.Path, orderAttr?.Order ?? 0, cmd.Shortcut);
        });
    }

    public IPopupCommand? CreateCommand(Type type)
    {
        IPopupCommand? command = Activator.CreateInstance(type) as IPopupCommand;
        return command;
    }
    
    public void RenderContextMenu(object? target)
    {
        if (target == null || _menuTreeRenderer == null) return;
        Type targetType = target.GetType();
        
        var matchingCommands = PopupCommands.Where(cmd => 
            !cmd.ExcludingTypes.Contains(targetType) && 
            (cmd.IsExactType ? cmd.TargetType == targetType : cmd.TargetType.IsAssignableFrom(targetType))
        );
        
        var menuDataList = matchingCommands.Select(cmd =>
        {
            var orderAttr = cmd.GetType().GetCustomAttribute<OrderAttribute>();
            int order = orderAttr?.Order ?? 0;
            
            ImGuiKey[] shortcuts = cmd.Shortcut ?? []; 

            return new CommandMenuData(cmd, cmd.Path, order, shortcuts);
        }).ToList();
        
        _menuTreeRenderer.RegenerateTree(menuDataList);
        _menuTreeRenderer.Draw(commandInstance =>
        {
            var popupCmd = (IPopupCommand)commandInstance;
            EditorUI.UiContext.EnqueueCommand(popupCmd);
        });
    }
}
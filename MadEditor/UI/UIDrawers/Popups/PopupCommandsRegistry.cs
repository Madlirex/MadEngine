using ImGuiNET;

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
    internal readonly List<IPopupCommand> PopupCommands = [];
    
    public override void Initialize()
    {
        DiscoverCommands();
    }

    public void DiscoverCommands()
    {
        PopupCommands.Clear();
        var types = ScriptDomain.GetTypesImplementing(typeof(IPopupCommand));
        foreach (var type in types)
        {
            var command = CreateCommand(type);
            if(command != null) RegisterCommand(command);
        }
    }

    public void RegisterCommand(IPopupCommand command)
    {
        PopupCommands.Add(command);
    }

    public IPopupCommand? CreateCommand(Type type)
    {
        IPopupCommand? command = Activator.CreateInstance(type) as IPopupCommand;
        return command;
    }
    
    public void RenderContextMenu(object? target)
    {
        if (target == null) return;
        Type targetType = target.GetType();
        
        var matchingCommands = PopupCommands.Where(cmd => cmd.TargetType.IsAssignableFrom(targetType));

        var popupCommands = matchingCommands as IPopupCommand[] ?? matchingCommands.ToArray();
        foreach (var command in popupCommands)
        {
            Console.WriteLine(command.Path);
            string[] parts = command.Path.Split('/');
            RenderMenuRecursive(parts, 0, command, target);
        }
        
        if(popupCommands.ToArray().Length == 0) ImGui.TextDisabled("None"); 
    }

    internal void RenderMenuRecursive(string[] parts, int index, IPopupCommand command, object target)
    {
        if (index == parts.Length - 1)
        {
            if (ImGui.MenuItem(parts[index]))
            {
                command.Execute(target);
            }
            return;
        }

        if (!ImGui.BeginMenu(parts[index])) return;
        RenderMenuRecursive(parts, index + 1, command, target);
        ImGui.EndMenu();
    }
}
using System.Reflection;
using ImGuiNET;

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
    internal readonly List<MenubarCommand> MenubarCommands = [];
    
    public override void Initialize()
    {
        DiscoverCommands();
    }

    public void DiscoverCommands()
    {
        MenubarCommands.Clear();
        var types = ScriptDomain.GetTypesImplementing(typeof(MenubarCommand));
        foreach (var type in types)
        {
            var command = CreateCommand(type);
            if(command != null) RegisterCommand(command);
        }
    }

    public void RegisterCommand(MenubarCommand command)
    {
        MenubarCommands.Add(command);
    }

    public MenubarCommand? CreateCommand(Type type)
    {
        MenubarCommand? command = Activator.CreateInstance(type) as MenubarCommand;
        return command;
    }
    
    public void Draw(EditorUIContext context)
    {
        if (!ImGui.BeginMainMenuBar()) return;
        
        if (MenubarCommands.Count == 0)
        {
            ImGui.TextDisabled("None");
        }
        
        foreach (var command in MenubarCommands)
        {
            string[] parts = command.Path.Split('/');
            RenderMenuRecursive(parts, 0, command);
        }
        
        ImGui.EndMainMenuBar();
    }

    internal void RenderMenuRecursive(string[] parts, int index, MenubarCommand command)
    {
        if (index == parts.Length - 1)
        {
            if (ImGui.MenuItem(parts[index]))
            {
                EditorUI.UiContext.EnqueueCommand(command);
            }
            return;
        }

        if (!ImGui.BeginMenu(parts[index])) return;
        RenderMenuRecursive(parts, index + 1, command);
        ImGui.EndMenu();
    }
}
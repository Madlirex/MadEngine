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
    private readonly List<MenuNode> _menuTreeRoot = [];

    private class MenuNode
    {
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        
        public ImGuiKey[] ShortcutKeys { get; set; } = [];
        public string ShortcutText { get; set; } = string.Empty;
        
        public MenubarCommand? Command { get; set; }
        public List<MenuNode> Children { get; } = [];
        public bool IsLeaf => Command != null;
    }
    
    public override void Initialize()
    {
        DiscoverCommands();
    }

    public void DiscoverCommands()
    {
        _menuTreeRoot.Clear();

        var flatCommands = new List<(MenubarCommand Command, int Order)>();
        
        var types = ScriptDomain.GetTypesImplementing(typeof(MenubarCommand));
        foreach (var type in types)
        {
            if (Activator.CreateInstance(type) is not MenubarCommand command) continue;
            
            var orderAttr = type.GetCustomAttribute<OrderAttribute>();
            var order = orderAttr?.Order ?? 0;
            flatCommands.Add((command, order));
        }

        BuildTree(flatCommands);
    }
    
    public MenubarCommand? CreateCommand(Type type)
    {
        return Activator.CreateInstance(type) as MenubarCommand;
    }
    
    public void RegisterCommand(MenubarCommand command)
    {
        var orderAttr = command.GetType().GetCustomAttribute<OrderAttribute>();
        int order = orderAttr?.Order ?? 0;
        
        BuildTree([(command, order)]); 
    }

    public void BuildTree(List<(MenubarCommand Command, int Order)> commands)
    {
        foreach (var (command, order) in commands)
        {
            string[] parts = command.Path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            List<MenuNode> currentLevel = _menuTreeRoot;

            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i];
                bool isLast = i == parts.Length - 1;
                
                var existingNode = currentLevel.FirstOrDefault(n => n.Name.Equals(part, StringComparison.Ordinal));

                if (existingNode == null)
                {
                    var newNode = new MenuNode
                    {
                        Name = part,
                        Order = isLast ? order : 0
                    };

                    if (isLast)
                    {
                        newNode.Command = command;
                        newNode.ShortcutKeys = command.Shortcut;
                        newNode.ShortcutText = ShortcutUtility.ToDisplayString(command.Shortcut);
                    }

                    currentLevel.Add(newNode);
                    existingNode = newNode;
                }
                else if (isLast)
                {
                    existingNode.Command = command;
                    existingNode.Order = order;
                    existingNode.ShortcutKeys = command.Shortcut;
                    existingNode.ShortcutText = ShortcutUtility.ToDisplayString(command.Shortcut);
                }

                currentLevel = existingNode.Children;
            }
        }
        SortTreeRecursive(_menuTreeRoot);
    }
    
    private void SortTreeRecursive(List<MenuNode> nodes)
    {
        if (nodes.Count == 0) return;
        
        nodes.Sort((a, b) => a.Order.CompareTo(b.Order));
        
        foreach (var node in nodes)
        {
            SortTreeRecursive(node.Children);
        }
    }
    
    public void Draw(EditorUIContext context)
    {
        ProcessShortcuts(context);
        if (!ImGui.BeginMainMenuBar()) return;

        if (_menuTreeRoot.Count == 0)
        {
            ImGui.TextDisabled("None");
        }
        else
        {
            RenderMenuLevel(_menuTreeRoot, context);
        }

        ImGui.EndMainMenuBar();
    }

    private void RenderMenuLevel(List<MenuNode> nodes, EditorUIContext context)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            var currentNode = nodes[i];

            if (currentNode.IsLeaf)
            {
                if (ImGui.MenuItem(currentNode.Name, currentNode.ShortcutText))
                {
                    context.EnqueueCommand(currentNode.Command!);
                }
            }
            else
            {
                if (ImGui.BeginMenu(currentNode.Name))
                {
                    RenderMenuLevel(currentNode.Children, context);
                    ImGui.EndMenu();
                }
            }

            if (i >= nodes.Count - 1) continue;
            var nextNode = nodes[i + 1];
            
            int currentBucket = (int)Math.Floor((double)currentNode.Order / 1000);
            int nextBucket = (int)Math.Floor((double)nextNode.Order / 1000);

            if (currentBucket == nextBucket) continue;
            int separatorCount = Math.Abs(nextBucket - currentBucket);
            for (int s = 0; s < separatorCount; s++)
            {
                ImGui.Separator();
            }
        }
    }
    
    private IEnumerable<MenuNode> FlatLeafNodes(List<MenuNode> nodes)
    {
        foreach (var node in nodes)
        {
            if (node.IsLeaf) yield return node;
            foreach (var childLeaf in FlatLeafNodes(node.Children))
            {
                yield return childLeaf;
            }
        }
    }
    
    public void ProcessShortcuts(EditorUIContext context)
    {
        if (ImGui.GetIO().WantTextInput) return;

        foreach (var node in FlatLeafNodes(_menuTreeRoot))
        {
            if (!ShortcutInputEngine.IsShortcutPressed(node.ShortcutKeys)) continue;
            context.EnqueueCommand(node.Command!);
            break;
        }
    }
}
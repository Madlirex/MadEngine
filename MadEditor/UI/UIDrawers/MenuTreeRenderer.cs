using System;
using System.Collections.Generic;
using System.Linq;
using ImGuiNET;

namespace MadEditor;

internal record CommandMenuData(
    object CommandInstance, 
    string Path, 
    int Order, 
    ImGuiKey[] ShortcutKeys
);

internal class MenuTreeRenderer
{
    private class MenuNode
    {
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public ImGuiKey[] ShortcutKeys { get; set; } = [];
        public string ShortcutText { get; set; } = string.Empty;
        public object? CommandInstance { get; set; }
        public List<MenuNode> Children { get; } = [];
        public bool IsLeaf => CommandInstance != null;
    }

    private readonly List<MenuNode> _treeRoot = [];
    private readonly Dictionary<string, int> _categoryWeights;

    public MenuTreeRenderer(Dictionary<string, int> categoryWeights)
    {
        _categoryWeights = categoryWeights;
    }

    public void RegenerateTree(IEnumerable<CommandMenuData> flatCommands)
    {
        _treeRoot.Clear();

        foreach (var data in flatCommands)
        {
            string[] parts = data.Path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            List<MenuNode> currentLevel = _treeRoot;
            string currentFullPath = string.Empty;

            for (int i = 0; i < parts.Length; i++)
            {
                string part = parts[i];
                bool isLast = i == parts.Length - 1;
                currentFullPath = i == 0 ? part : $"{currentFullPath}/{part}";

                var existingNode = currentLevel.FirstOrDefault(n => n.Name.Equals(part, StringComparison.Ordinal));

                if (existingNode == null)
                {
                    int defaultFallback = isLast ? data.Order : 0;
                    int nodeOrder = _categoryWeights.GetValueOrDefault(currentFullPath, defaultFallback);

                    var newNode = new MenuNode
                    {
                        Name = part,
                        Order = nodeOrder
                    };

                    if (isLast)
                    {
                        newNode.CommandInstance = data.CommandInstance;
                        newNode.ShortcutKeys = data.ShortcutKeys;
                        newNode.ShortcutText = ShortcutUtility.ToDisplayString(data.ShortcutKeys);
                    }

                    currentLevel.Add(newNode);
                    existingNode = newNode;
                }
                else if (isLast)
                {
                    existingNode.CommandInstance = data.CommandInstance;
                    existingNode.Order = data.Order;
                    existingNode.ShortcutKeys = data.ShortcutKeys;
                    existingNode.ShortcutText = ShortcutUtility.ToDisplayString(data.ShortcutKeys);
                }

                currentLevel = existingNode.Children;
            }
        }

        SortTreeRecursive(_treeRoot);
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

    public void Draw(Action<object> onCommandTriggered)
    {
        if (_treeRoot.Count == 0)
        {
            ImGui.TextDisabled("None");
            return;
        }

        RenderMenuLevel(_treeRoot, onCommandTriggered);
    }

    private void RenderMenuLevel(List<MenuNode> nodes, Action<object> onCommandTriggered)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            var currentNode = nodes[i];

            if (currentNode.IsLeaf)
            {
                if (ImGui.MenuItem(currentNode.Name, currentNode.ShortcutText))
                {
                    onCommandTriggered(currentNode.CommandInstance!);
                }
            }
            else
            {
                if (ImGui.BeginMenu(currentNode.Name))
                {
                    RenderMenuLevel(currentNode.Children, onCommandTriggered);
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
                ImGui.Separator();
        }
    }
}

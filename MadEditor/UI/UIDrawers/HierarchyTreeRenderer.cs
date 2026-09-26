using ImGuiNET;

namespace MadEditor;

internal record TreeNodeData<T>(
    string Path, 
    int Order,
    T Instance
);

internal class HierarchyTreeRenderer<T> where T : class
{
    public T? SelectedInstance;
    
    private class TreeNode
    {
        public required T Instance;
        
        public string Name { get; set; } = string.Empty;

        public int Order
        {
            get => _order;
            set
            {
                UseMultiSeparators = value < 0;
                _order = UseMultiSeparators ? -value : value;
            }
        }

        private int _order;
        public bool UseMultiSeparators { get; private set; }
        public bool IsLeaf;
        public List<TreeNode> Children { get; } = [];
    }

    private readonly List<TreeNode> _treeRoot = [];
    private readonly Dictionary<string, int> _categoryWeights;

    public HierarchyTreeRenderer(Dictionary<string, int> categoryWeights)
    {
        _categoryWeights = categoryWeights;
    }

    public void RegenerateTree(IEnumerable<TreeNodeData<T>> flatTabs)
    {
        _treeRoot.Clear();

        foreach (var data in flatTabs)
        {
            string[] parts = data.Path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            List<TreeNode> currentLevel = _treeRoot;
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

                    var newNode = new TreeNode()
                    {
                        Name = part,
                        Order = nodeOrder,
                        IsLeaf = isLast,
                        Instance = data.Instance
                    };

                    currentLevel.Add(newNode);
                    existingNode = newNode;
                }
                else if (isLast)
                {
                    existingNode.Order = data.Order;
                    existingNode.Instance = data.Instance;
                }

                currentLevel = existingNode.Children;
            }
        }

        SortTreeRecursive(_treeRoot);
    }

    private void SortTreeRecursive(List<TreeNode> nodes)
    {
        if (nodes.Count == 0) return;
        
        nodes.Sort((a, b) => a.Order.CompareTo(b.Order));
        
        foreach (var node in nodes)
        {
            SortTreeRecursive(node.Children);
        }
    }

    public void Draw()
    {
        if (_treeRoot.Count == 0)
        {
            ImGui.TextDisabled("None");
            return;
        }

        RenderMenuLevel(_treeRoot);
    }

    private void RenderMenuLevel(List<TreeNode> nodes)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            var currentNode = nodes[i];
            
            if (currentNode.IsLeaf)
            {
                bool isSelected = SelectedInstance == currentNode.Instance;
                if (ImGui.Selectable(currentNode.Name, isSelected))
                {
                    SelectedInstance = currentNode.Instance;
                }
            }
            else
            {
                ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.OpenOnArrow | ImGuiTreeNodeFlags.OpenOnDoubleClick;
                
                if (ImGui.TreeNodeEx(currentNode.Name, flags))
                {
                    RenderMenuLevel(currentNode.Children);
                    ImGui.TreePop();
                }
            }

            if (i >= nodes.Count - 1) continue;
            var nextNode = nodes[i + 1];
            int currentBucket = (int)Math.Floor((double)currentNode.Order / 1000);
            int nextBucket = (int)Math.Floor((double)nextNode.Order / 1000);

            if (currentBucket == nextBucket) continue;
            int separatorCount = Math.Abs(nextBucket - currentBucket);
            
            if (!nextNode.UseMultiSeparators)
            {
                if(separatorCount != 0)
                {
                    ImGui.Separator();
                }
            }
            else
            {
                for (int s = 0; s < separatorCount; s++)
                {
                    ImGui.Separator();
                }
            }
        }
    }
}

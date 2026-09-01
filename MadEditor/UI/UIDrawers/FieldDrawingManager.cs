using System.Reflection;
using MadEngine.Core;

namespace MadEditor;

public static class FieldDrawingManager
{
    private static object? _currentTarget;
    private static Dictionary<object, List<InspectorMember>> _cachedMembers = new();

    public static void Render(object? target)
    {
        if(target != _currentTarget) OnSelectionChanged(target);
        OnRenderFrame(target);
    }

    public static void Render(object? target, object? parent)
    {
        if(parent != _currentTarget) OnSelectionChanged(parent);
        CreateMembers(target);
        OnRenderFrame(target);
    }
    
    public static void OnSelectionChanged(object? newTarget)
    {
        _currentTarget = newTarget;
        _cachedMembers.Clear();

        if (newTarget == null) return;

        CreateMembers(newTarget);
    }
    
    public static void OnRenderFrame(object? target)
    {
        if (target == null) return;
        
        List<InspectorMember> members = _cachedMembers.GetValueOrDefault(target) ?? CreateMembers(target);
        foreach(var member in members)
        {
            if (FieldDrawerRegistry.TryGetDrawer(member.Type, out var drawer))
            {
                drawer.Draw(target, member);
            }
        }
    }

    public static void RenderChild(object? target)
    {
        OnRenderFrame(target);
    }
    
    public static void OnRenderFrame(object? target, InspectorMember[] members)
    {
        if (target == null) return;
        if (members.Length == 0) return;

        foreach (var member in members)
        {
            if (FieldDrawerRegistry.TryGetDrawer(member.Type, out var drawer))
            {
                drawer.Draw(target, member);
            }
        }
    }

    public static List<InspectorMember> CreateMembers(object? target)
    {
        if(target == null) return [];
        if(_cachedMembers.TryGetValue(target, out var members)) return members;
        
        var membersQuery = InspectorMemberFactory.CreateMembers(target);

        _cachedMembers[target] = membersQuery.ToList();
        return _cachedMembers[target];
    }
}
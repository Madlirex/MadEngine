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

    public static List<InspectorMember> CreateMembers(object? target)
    {
        if(target == null) return [];
        if(_cachedMembers.TryGetValue(target, out var members)) return members;
        
        var membersQuery = target.GetType()
            .GetFields(BindingFlags.Instance | BindingFlags.Public)
            .Where(f => f.GetCustomAttribute<HideInInspectorAttribute>() == null)
            .Select(f => (InspectorMember)new FieldMember(f))
            .Concat(
                target.GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => p.GetCustomAttribute<ShowInInspectorAttribute>() != null)
                    .Select(p => (InspectorMember)new PropertyMember(p))
            ).Concat(
                target.GetType()
                    .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                    .Where(f => f.GetCustomAttribute<ShowInInspectorAttribute>() != null)
                    .Select(f => (InspectorMember)new FieldMember(f)))
            .OrderBy(m => m.Order);

        _cachedMembers[target] = membersQuery.ToList();
        return _cachedMembers[target];
    }
}
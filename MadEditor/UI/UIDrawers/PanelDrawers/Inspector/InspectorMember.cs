using System.Reflection;
using MadEngine.Core;

namespace MadEditor;

public abstract class InspectorMember
{
    public abstract Guid Guid { get; }
    public abstract string Name { get; }
    public abstract Type Type { get; }
    public abstract object? GetValue(object obj);
    public abstract void SetValue(object obj, object? value);
    public abstract int Order { get; }

    public override string ToString()
    {
        return $"{Name}##{Guid}";
    }
}

public class FieldMember : InspectorMember
{
    public override Guid Guid { get; }
    private readonly FieldInfo _field;
    public override string Name => _field.GetCustomName();

    public FieldMember(FieldInfo field)
    {
        Guid = Guid.NewGuid();
        _field = field;
    }

    public override Type Type => _field.FieldType;
    public override object? GetValue(object obj) => _field.GetValue(obj);
    public override void SetValue(object obj, object? value) => _field.SetValue(obj, value);
    public override int Order => _field.GetCustomAttribute<ShowInInspectorAttribute>()?.Order ?? 0;
}

public class PropertyMember : InspectorMember
{
    public override Guid Guid { get; }
    private readonly PropertyInfo _property;
    public override string Name => _property.GetCustomName();

    public PropertyMember(PropertyInfo property)
    {
        Guid = Guid.NewGuid();
        _property = property;
    }

    public override Type Type => _property.PropertyType;
    public override object? GetValue(object obj) => _property.GetValue(obj);
    public override void SetValue(object obj, object? value) => _property.SetValue(obj, value);
    public override int Order => _property.GetCustomAttribute<ShowInInspectorAttribute>()?.Order ?? 0;
}

public class CollectionElementMember : InspectorMember
{
    private readonly Func<object?> _getter;
    private readonly Action<object?> _setter;

    public override Guid Guid { get; }
    public override string Name { get; }
    public override Type Type { get; }
    public override int Order => 0;

    public CollectionElementMember(string name, Type type, Guid parentGuid, object elementIdentifier, Func<object?> getter, Action<object?> setter)
    {
        Name = name;
        Type = type;
        _getter = getter;
        _setter = setter;
        
        byte[] parentBytes = parentGuid.ToByteArray();
        int hash = elementIdentifier.GetHashCode();
        byte[] hashBytes = BitConverter.GetBytes(hash);
        for (int i = 0; i < hashBytes.Length; i++) 
            parentBytes[i] ^= hashBytes[i];
        Guid = new Guid(parentBytes);
    }

    public override object? GetValue(object obj) => _getter();
    public override void SetValue(object obj, object? value) => _setter(value);
}

public static class InspectorMemberFactory
{
    public static InspectorMember[] CreateMembers(object? target)
    {
        if(target == null) return [];
        
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

        return membersQuery.ToArray();
    }
}
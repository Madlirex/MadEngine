using System.Reflection;
using MadEngine.Core;

namespace MadEditor;

public abstract class InspectorMember
{
    public abstract string Name { get; }
    public abstract Type Type { get; }
    public abstract object? GetValue(object obj);
    public abstract void SetValue(object obj, object? value);
    public abstract int Order { get; }
}

public class FieldMember : InspectorMember
{
    private readonly FieldInfo _field;
    public override string Name => _field.Name;

    public FieldMember(FieldInfo field)
    {
        _field = field;
    }

    public override Type Type => _field.FieldType;
    public override object? GetValue(object obj) => _field.GetValue(obj);
    public override void SetValue(object obj, object? value) => _field.SetValue(obj, value);
    public override int Order => _field.GetCustomAttribute<ShowInInspectorAttribute>()?.Order ?? 0;
}

public class PropertyMember : InspectorMember
{
    private readonly PropertyInfo _property;
    public override string Name => _property.Name;

    public PropertyMember(PropertyInfo property)
    {
        _property = property;
    }

    public override Type Type => _property.PropertyType;
    public override object? GetValue(object obj) => _property.GetValue(obj);
    public override void SetValue(object obj, object? value) => _property.SetValue(obj, value);
    public override int Order => _property.GetCustomAttribute<ShowInInspectorAttribute>()?.Order ?? 0;
}
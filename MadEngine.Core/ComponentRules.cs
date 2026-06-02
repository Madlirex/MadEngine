using System.Reflection;

namespace MadEngine.Core;

public static class ComponentRules
{
    public static bool CanBeAdded(Type type)
    {
        var attr = type.GetCustomAttribute<CanBeAddedAttribute>();

        return attr?.Value ?? true;
    }

    public static bool CanBeRemoved(Type type)
    {
        var attr = type.GetCustomAttribute<CanBeRemovedAttribute>();

        return attr?.Value ?? true;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class CanBeAddedAttribute : Attribute
{
    public bool Value;

    public CanBeAddedAttribute(bool value)
    {
        Value = value;
    }

}

[AttributeUsage(AttributeTargets.Class)]
public class CanBeRemovedAttribute : Attribute
{
    public bool Value;

    public CanBeRemovedAttribute(bool value)
    {
        Value = value;
    }

}
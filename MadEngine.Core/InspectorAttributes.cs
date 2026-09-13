namespace MadEngine.Core;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ShowInInspectorAttribute : Attribute;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class HideInInspectorAttribute : Attribute;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)]
public class CustomNameAttribute(string name) : Attribute
{
    public string Name = name;
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, Inherited = false)]
public class OrderAttribute(int order = 0) : Attribute
{
    public int Order { get; } = order;
}
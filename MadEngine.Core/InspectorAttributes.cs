namespace MadEngine.Core;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ShowInInspectorAttribute(int order = 0) : Attribute
{
    public int Order = order;
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class HideInInspectorAttribute : Attribute;
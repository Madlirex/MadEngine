using System.Reflection;
using ImGuiNET;
using MadEngine.Core;
using System.Numerics;

namespace MadEditor;

public abstract class FieldDrawer
{
    public abstract void Draw(object target, FieldInfo field);
}

[AttributeUsage(AttributeTargets.Class)]
public class CustomFieldDrawerAttribute : Attribute
{
    public Type TargetType { get; }
    
    public CustomFieldDrawerAttribute(Type targetType)
    {
        TargetType = targetType;
    }
}

public static class FieldDrawerRegistry
{
    private static readonly Dictionary<Type, FieldDrawer> _drawers = new();

    public static void Initialize()
    {
        foreach (Type type in AppDomain.CurrentDomain.GetAssemblies()
                     .SelectMany(a => a.GetTypes()))
        {
            if (type.IsAbstract)
                continue;

            if (!typeof(FieldDrawer).IsAssignableFrom(type))
                continue;

            var attribute = type.GetCustomAttribute<CustomFieldDrawerAttribute>();

            if (attribute == null)
                continue;

            FieldDrawer drawer = (FieldDrawer)Activator.CreateInstance(type)!;

            _drawers[attribute.TargetType] = drawer;
        }
    }

    public static bool TryGetDrawer(Type type, out FieldDrawer drawer)
    {
        return _drawers.TryGetValue(type, out drawer!);
    }
}

[CustomFieldDrawer(typeof(float))]
public class FloatDrawer : FieldDrawer
{
    public override void Draw(object target, FieldInfo field)
    {
        float value = (float)field.GetValue(target)!;

        if (ImGui.DragFloat(field.Name, ref value))
        {
            field.SetValue(target, value);
        }
    }
}

[CustomFieldDrawer(typeof(OpenTK.Mathematics.Vector3))]
public class Vector3Drawer : FieldDrawer
{
    public override void Draw(object target, FieldInfo field)
    {
        Vector3 value = MathFunctions.ToNumerics3((OpenTK.Mathematics.Vector3)field.GetValue(target)!);

        if (ImGui.DragFloat3(field.Name, ref value))
        {
            field.SetValue(target, MathFunctions.ToOtk3(value));
        }
    }
}

[CustomFieldDrawer(typeof(OpenTK.Mathematics.Vector4))]
public class Vector4Drawer : FieldDrawer
{
    public override void Draw(object target, FieldInfo field)
    {
        Vector4 value = MathFunctions.ToNumerics4((OpenTK.Mathematics.Vector4)field.GetValue(target)!);

        if (ImGui.DragFloat4(field.Name, ref value))
        {
            field.SetValue(target, MathFunctions.ToOtk4(value));
        }
    }
}
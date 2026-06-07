using System.Reflection;
using ImGuiNET;
using MadEngine.Core;
using System.Numerics;

namespace MadEditor;

public abstract class FieldDrawer
{
    public abstract void Draw(object target, InspectorMember member, Component component);
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
    private static readonly Dictionary<Type, FieldDrawer> Drawers = new();

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

            Drawers[attribute.TargetType] = drawer;
        }
    }

    public static bool TryGetDrawer(Type type, out FieldDrawer drawer)
    {
        Type? current = type;

        while (current != null)
        {
            if (Drawers.TryGetValue(current, out var found))
            {
                drawer = found;
                return true;
            }

            current = current.BaseType;
        }

        drawer = null!;
        return false;
    }
}

[CustomFieldDrawer(typeof(float))]
public class FloatDrawer : FieldDrawer
{
    public override void Draw(object target, InspectorMember member, Component component)
    {
        ImGui.PushID(component + member.Name);
        
        float value = (float)member.GetValue(target)!;

        if (ImGui.DragFloat(member.Name, ref value))
        {
            member.SetValue(target, value);
        }
        
        ImGui.PopID();
    }
}

[CustomFieldDrawer(typeof(OpenTK.Mathematics.Vector3))]
public class Vector3Drawer : FieldDrawer
{
    public override void Draw(object target, InspectorMember member, Component component)
    {
        ImGui.PushID(component + member.Name);
        Vector3 value = MathFunctions.ToNumerics3((OpenTK.Mathematics.Vector3)member.GetValue(target)!);

        if (ImGui.DragFloat3(member.Name, ref value))
        {
            member.SetValue(target, MathFunctions.ToOtk3(value));
        }
        ImGui.PopID();
    }
}

[CustomFieldDrawer(typeof(OpenTK.Mathematics.Vector4))]
public class Vector4Drawer : FieldDrawer
{
    public override void Draw(object target, InspectorMember member, Component component)
    {
        ImGui.PushID(component + member.Name);
        Vector4 value = MathFunctions.ToNumerics4((OpenTK.Mathematics.Vector4)member.GetValue(target)!);

        if (ImGui.DragFloat4(member.Name, ref value))
        {
            member.SetValue(target, MathFunctions.ToOtk4(value));
        }
        ImGui.PopID();
    }
}

[CustomFieldDrawer(typeof(OpenTK.Mathematics.Quaternion))]
public class QuaternionDrawer : FieldDrawer
{
    public override void Draw(object target, InspectorMember member, Component component)
    {
        ImGui.PushID(component + member.Name);
        Vector3 value = MathFunctions.ToNumerics3((OpenTK.Mathematics.Quaternion)member.GetValue(target)!);

        if (ImGui.DragFloat3(member.Name, ref value))
        {
            member.SetValue(target, MathFunctions.ToQuaternion(value));
        }
        ImGui.PopID();
    }
}

[CustomFieldDrawer(typeof(Component))]
public class ComponentDrawer : FieldDrawer
{
    public override void Draw(object target, InspectorMember member, Component component)
    {
        ImGui.PushID(component + member.Name);
        
        ImGui.Text(member.Name + member.Type.Name);
        
        ImGui.PopID();
    }
}

[CustomFieldDrawer(typeof(bool))]
public class BoolDrawer : FieldDrawer
{
    public override void Draw(object target, InspectorMember member, Component component)
    {
        ImGui.PushID(component + member.Name);

        bool value = (bool)member.GetValue(target)!;

        if (ImGui.Checkbox(member.Name, ref value))
        {
            member.SetValue(target, value);
        }

        ImGui.PopID();
    }
}

[CustomFieldDrawer(typeof(int))]
public class IntDrawer : FieldDrawer
{
    public override void Draw(object target, InspectorMember member, Component component)
    {
        ImGui.PushID(component + member.Name);

        int value = (int)member.GetValue(target)!;

        if (ImGui.DragInt(member.Name, ref value))
        {
            member.SetValue(target, value);
        }

        ImGui.PopID();
    }
}

[CustomFieldDrawer(typeof(string))]
public class StringDrawer : FieldDrawer
{
    public override void Draw(object target, InspectorMember member, Component component)
    {
        ImGui.PushID(component + member.Name);

        string value = (string?)member.GetValue(target) ?? "";

        byte[] buffer = new byte[256];
        System.Text.Encoding.UTF8.GetBytes(value, 0, value.Length, buffer, 0);

        if (ImGui.InputText(member.Name, buffer, (uint)buffer.Length))
        {
            string newValue = System.Text.Encoding.UTF8.GetString(buffer).TrimEnd('\0');

            member.SetValue(target, newValue);
        }

        ImGui.PopID();
    }
}
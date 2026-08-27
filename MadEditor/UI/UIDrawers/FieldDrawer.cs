using System.Collections;
using System.Reflection;
using ImGuiNET;
using MadEngine.Core;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace MadEditor;

public abstract class FieldDrawer
{
    public abstract void Draw(object target, InspectorMember member);
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

public static class FieldDrawerManager
{
    public static void Draw(object target, FieldDrawer drawer, InspectorMember member)
    {
        ImGui.PushID(member.Name);
        
        drawer.Draw(target, member);
        
        ImGui.PopID();
    }
}

public static class FieldDrawerRegistry
{
    private static FieldDrawerEngine Instance => RegistryBootstrapper.Get<FieldDrawerEngine>();
    
    public static bool TryGetDrawer(Type type, out FieldDrawer drawer) => Instance.TryGetDrawer(type, out drawer);
}

internal class FieldDrawerEngine : Registry
{
    private readonly Dictionary<Type, FieldDrawer> _drawers = [];

    public override void Initialize()
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

    internal bool TryGetDrawer(Type type, out FieldDrawer drawer)
    {
        Type? current = type;

        while (current != null)
        {
            if (_drawers.TryGetValue(current, out var found))
            {
                drawer = found;
                return true;
            }
            
            if (current.GetInterfaces().Any(interfaceType => _drawers.TryGetValue(interfaceType, out found) || (interfaceType.IsGenericType &&
                    _drawers.TryGetValue(
                        interfaceType.GetGenericTypeDefinition(),
                        out found))))
            {
                drawer = found!;
                return true;
            }
            
            if (current.IsGenericType && _drawers.TryGetValue(current.GetGenericTypeDefinition(), out found))
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
    public override void Draw(object target, InspectorMember member)
    {
        float value = (float)member.GetValue(target)!;

        if (ImGui.DragFloat(member.GetCustomName(), ref value))
        {
            member.SetValue(target, value);
        }
    }
}

[CustomFieldDrawer(typeof(OpenTK.Mathematics.Vector3))]
public class Vector3Drawer : FieldDrawer
{
    public override void Draw(object target, InspectorMember member)
    {
        Vector3 value = MathFunctions.ToNumerics3((OpenTK.Mathematics.Vector3)member.GetValue(target)!);

        if (ImGui.DragFloat3(member.GetCustomName(), ref value))
        {
            member.SetValue(target, MathFunctions.ToOtk3(value));
        }
    }
}

[CustomFieldDrawer(typeof(OpenTK.Mathematics.Vector4))]
public class Vector4Drawer : FieldDrawer
{
    public override void Draw(object target, InspectorMember member)
    {
        Vector4 value = MathFunctions.ToNumerics4((OpenTK.Mathematics.Vector4)member.GetValue(target)!);

        if (ImGui.DragFloat4(member.GetCustomName(), ref value))
        {
            member.SetValue(target, MathFunctions.ToOtk4(value));
        }
    }
}

[CustomFieldDrawer(typeof(OpenTK.Mathematics.Quaternion))]
public class QuaternionDrawer : FieldDrawer
{
    public override void Draw(object target, InspectorMember member)
    {
        Vector3 value = MathFunctions.ToNumerics3((OpenTK.Mathematics.Quaternion)member.GetValue(target)!);

        if (ImGui.DragFloat3(member.GetCustomName(), ref value))
        {
            member.SetValue(target, MathFunctions.ToQuaternion(value));
        }
    }
}

[CustomFieldDrawer(typeof(Component))]
public class ComponentDrawer : FieldDrawer
{
    public override void Draw(object target, InspectorMember member)
    {
        ImGui.Text(member.GetCustomName() + " " + member.Type.Name);
    }
}

[CustomFieldDrawer(typeof(bool))]
public class BoolDrawer : FieldDrawer
{
    public override void Draw(object target, InspectorMember member)
    {
        bool value = (bool)member.GetValue(target)!;

        if (ImGui.Checkbox(member.GetCustomName(), ref value))
        {
            member.SetValue(target, value);
        }
    }
}

[CustomFieldDrawer(typeof(int))]
public class IntDrawer : FieldDrawer
{
    public override void Draw(object target, InspectorMember member)
    {
        int value = (int)member.GetValue(target)!;

        if (ImGui.DragInt(member.GetCustomName(), ref value))
        {
            member.SetValue(target, value);
        }
    }
}

[CustomFieldDrawer(typeof(uint))]
public class UIntDrawer : FieldDrawer
{
    public override void Draw(object target, InspectorMember member)
    {
        int value = Convert.ToInt32(member.GetValue(target)!);

        if (ImGui.DragInt(member.GetCustomName(), ref value))
        {
            member.SetValue(target, Unsafe.As<int, uint>(ref value));
        }
    }
}

[CustomFieldDrawer(typeof(string))]
public class StringDrawer : FieldDrawer
{
    public override void Draw(object target, InspectorMember member)
    {
        string value = (string?)member.GetValue(target) ?? "";

        byte[] buffer = new byte[256];
        System.Text.Encoding.UTF8.GetBytes(value, 0, value.Length, buffer, 0);

        if (ImGui.InputText(member.GetCustomName(), buffer, (uint)buffer.Length))
        {
            string newValue = System.Text.Encoding.UTF8.GetString(buffer).TrimEnd('\0');

            member.SetValue(target, newValue);
        }
    }
}

[CustomFieldDrawer(typeof(MadObject))]
public class MadObjectDrawer : FieldDrawer
{
    private readonly ReferenceSelectionPopup _popup = new();
    
    public override void Draw(object target, InspectorMember member)
    {
        string label = member.GetCustomName();
        MadObject? value = (MadObject?)member.GetValue(target);
        string text = value?.Name ?? $"None ({member.Type.GetCustomName()})";
        
        ImGuiInputTextFlags flags = ImGuiInputTextFlags.ReadOnly | ImGuiInputTextFlags.NoHorizontalScroll;
        
        ImGui.InputText(label, ref text, (uint)text.Length + 1, flags);
        
        if (DragDrop.TryAcceptTarget(member.Type, out var droppedObj))
        {
            if (droppedObj != null && member.Type.IsInstanceOfType(droppedObj))
            {
                member.SetValue(target, droppedObj);
            }
        }
        
        if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
        {
            _popup.Type = member.Type;
            _popup.Selected = value;
            
            _popup.OnObjectSelected = selectedObj => 
            {
                member.SetValue(target, selectedObj);
            };

            _popup.Open();
        }
        
        _popup.Draw(EditorUI.UiContext);
    }
}

[CustomFieldDrawer(typeof(IList))]
public class ListDrawer : FieldDrawer
{
    public override void Draw(object target, InspectorMember member)
    {
        IList? list = member.GetValue(target) as IList;
        if (list == null) return;
    
        Type listType = list.GetType();

        Type elementType = listType.IsArray ? listType.GetElementType()! : listType.GetGenericArguments()[0];
        if (!FieldDrawerRegistry.TryGetDrawer(elementType, out var elementDrawer)) return;
        string label = $"{member.Name} [{list.Count}]";
        
        if (ImGui.TreeNode(member.ToString(), label))
        {
            int currentSize = list.Count;
            if (ImGui.InputInt($"Size##{member.Guid}", ref currentSize))
            {
                if (currentSize < 0) currentSize = 0;
                ResizeList(ref list, member, target, currentSize);
            }
        
            for (int i = 0; i < list.Count; i++)
            {
                int index = i;

                var elementMember = new CollectionElementMember(
                    name: $"Element {index}",
                    type: elementType,
                    parentGuid: member.Guid,
                    elementIdentifier: index,
                    getter: () => list[index],
                    setter: val => list[index] = val
                );
                
                ImGui.PushID(elementMember.Guid.GetHashCode());

                if (elementDrawer != null)
                {
                    elementDrawer.Draw(list, elementMember);
                }
                else
                {
                    ImGui.Text($"No drawer for {elementType.Name} at [{index}]");
                }
                
                ImGui.PopID();
            }

            ImGui.TreePop();
        }
    }

    private void ResizeList(ref IList list, InspectorMember member, object target, int newSize)
    {
        Type listType = list.GetType();

        if (listType.IsArray)
        {
            Type elementType = listType.GetElementType()!;
            Array newArray = Array.CreateInstance(elementType, newSize);
            Array.Copy((Array)list, newArray, Math.Min(list.Count, newSize));
            member.SetValue(target, newArray);
        }
        else
        {
            while (list.Count < newSize)
            {
                Type elementType = listType.GetGenericArguments()[0];
                object? defaultValue = elementType.IsValueType ? Activator.CreateInstance(elementType) : null;
                list.Add(defaultValue);
            }
            while (list.Count > newSize)
            {
                list.RemoveAt(list.Count - 1);
            }
        }
    }
}
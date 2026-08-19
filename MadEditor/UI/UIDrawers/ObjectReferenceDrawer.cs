using System.Numerics;
using ImGuiNET;
using MadEngine.Core;

namespace MadEditor;

[CustomFieldDrawer(typeof(MadObject))]
public class ObjectReferenceDrawer : FieldDrawer
{
    public override void Draw(object target, InspectorMember member)
    {
        MadObject? currentRef = (MadObject?)member.GetValue(target);

        string labelText = currentRef != null
            ? currentRef.Name
            : $"None ({member.Type.Name})";

        float totalWidth = ImGui.GetContentRegionAvail().X;
        float pickerButtonWidth = 24.0f;
        float spacing = ImGui.GetStyle().ItemSpacing.X;

        float labelWidth = totalWidth * 0.40f;
        float fieldWidth = totalWidth - labelWidth - pickerButtonWidth - spacing;
        
        ImGui.TextUnformatted(member.GetCustomName());
        
        ImGui.SameLine();
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + labelWidth);

        if (ImGui.Button(
                $"{labelText}##{member.Guid}_slot",
                new Vector2(fieldWidth, ImGui.GetFrameHeight())))
        {
            Console.WriteLine("REFERENCE FIELD CLICKED");
            ImGui.OpenPopup($"ObjectPicker_{member.Guid}");
        }

        /*Console.WriteLine(
            $"Hovered={ImGui.IsItemHovered()} " +
            $"Active={ImGui.IsItemActive()} " +
            $"MouseDown={ImGui.GetIO().MouseDown[0]}");*/
        
        DragPayload.DropTarget(DragPayload.MadObjectType, draggedId =>
        {
            Console.WriteLine("hoho");
            MadObject? droppedObject = AssetRegistry.GetObject(draggedId);
            if (droppedObject == null) return;

            if (!member.Type.IsAssignableFrom(droppedObject.GetType()))
            {
                if (member.Type.IsAssignableFrom(typeof(Component)) && droppedObject is GameObject obj)
                {
                    droppedObject = obj.GetComponent(member.Type);
                }
                else return;
            }

            member.SetValue(target, droppedObject);
        });
        
        ImGui.SameLine();

        if (ImGui.Button(
                $"O##{member.Guid}_picker",
                new Vector2(
                    pickerButtonWidth,
                    ImGui.GetFrameHeight())))
        {
            Console.WriteLine("O BUTTON CLICKED");
            ImGui.OpenPopup($"ObjectPicker_{member.Guid}");
        }
        
        if (ImGui.BeginPopup($"ObjectPicker_{member.Guid}"))
        {
            ImGui.Text($"Select {member.Type.Name}");
            ImGui.Separator();

            if (ImGui.Selectable("None", currentRef == null))
            {
                member.SetValue(target, null);
                ImGui.CloseCurrentPopup();
            }

            foreach (var obj in AssetRegistry.GetObjectsImplementing(member.Type))
            {
                if (ImGui.Selectable(obj.Name, currentRef == obj))
                {
                    member.SetValue(target, obj);
                    ImGui.CloseCurrentPopup();
                }
            }

            ImGui.EndPopup();
        }
    }
}
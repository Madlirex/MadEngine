using ImGuiNET;

namespace MadEditor;

public interface IEditorCommand
{
    public void Execute(object target);
}

public interface IHasShortcut
{
    ImGuiKey[] Shortcut { get; }
}
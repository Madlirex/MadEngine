using ImGuiNET;

namespace MadEditor;

public abstract class MenubarCommand : IEditorCommand
{
    public virtual ImGuiKey[] Shortcut => [];
    public abstract string Path { get; set; }
    public abstract void Execute(EditorUIContext context);
    void IEditorCommand.Execute(object target) => Execute((EditorUIContext)target);
}
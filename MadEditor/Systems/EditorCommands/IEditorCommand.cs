namespace MadEditor;

public interface IEditorCommand
{
    public void Execute(EditorUIContext editorUIContext);
}
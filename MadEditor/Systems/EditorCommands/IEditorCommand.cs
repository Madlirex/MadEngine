namespace MadEditor;

public interface IEditorCommand
{
    public void Execute(object target);
}
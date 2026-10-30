namespace MadEditor.DoubleClick;

public interface IDoubleClickCommand : IEditorCommand
{
    Type TargetType { get; }
}

public abstract class DoubleClickCommand<T> : IDoubleClickCommand
{
    public Type TargetType => typeof(T);

    void IEditorCommand.Execute(object target) => Execute((T)target);

    public abstract void Execute(T target);
}
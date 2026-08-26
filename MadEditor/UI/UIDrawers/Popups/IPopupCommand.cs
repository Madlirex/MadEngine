using MadEngine.Core;

namespace MadEditor;

public interface IPopupCommand : IEditorCommand
{
    bool IsExactType { get; }
    Type[] ExcludingTypes { get; }
    Type TargetType { get; }
    string Path { get; }

}

public abstract class PopupCommand<T> : IPopupCommand where T : class
{
    public virtual bool IsExactType => false;
    public virtual Type[] ExcludingTypes => [];
    public Type TargetType => typeof(T);
    public abstract string Path { get; }

    public abstract void Execute(T target);
    
    void IEditorCommand.Execute(object target) => Execute((T)target);
}
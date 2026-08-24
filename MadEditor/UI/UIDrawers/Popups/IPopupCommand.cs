using MadEngine.Core;

namespace MadEditor;

public interface IPopupCommand
{
    Type TargetType { get; }
    string Path { get; }

}

public abstract class PopupCommand<T> : IPopupCommand, IEditorCommand where T : class
    {
        public Type TargetType => typeof(T);
        public abstract string Path { get; }

        public abstract void Execute(EditorUIContext context);
        
        void IEditorCommand.Execute(EditorUIContext context) => Execute(context);
    }
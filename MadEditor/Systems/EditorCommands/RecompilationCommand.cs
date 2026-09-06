using MadEngine.Core.SceneManagement;

namespace MadEditor;

public class RecompilationCommand : IEditorCommand
{
    public void Execute(object target)
    {
        AssetManager.RecompileScripts();
        
        EditorUI.UiContext.Engine.EditorStart(SceneManager.ActiveScene);
    }
}
using MadEngine.Core;
using MadEngine.Core.SceneManagement;

namespace MadEditor;

public class RecompilationCommand : IEditorCommand
{
    public void Execute(object target)
    {
        DateTime start = DateTime.Now;
        AssetManager.RecompileScripts();
        DateTime end = DateTime.Now;
        
        Debug.Log($"Compilation finished in: {end - start}");
        
        EditorUI.UiContext.Engine.EditorStart(SceneManager.ActiveScene);
        
    }
}
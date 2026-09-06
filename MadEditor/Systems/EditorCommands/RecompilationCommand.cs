using MadEngine.Core;
using MadEngine.Core.SceneManagement;

namespace MadEditor;

public class RecompilationCommand : IEditorCommand
{
    public void Execute(object target)
    {
        DateTime start = DateTime.Now;

        Guid? selectedGuid = EditorUI.UiContext.Selected?.Guid;
        
        AssetManager.RecompileScripts();
        DateTime end = DateTime.Now;

        if (selectedGuid.HasValue)
            EditorUI.UiContext.Selected = AssetRegistry.GetObject(selectedGuid.Value);
        
        Debug.Log($"Compilation finished in: {end - start}");
        
        EditorUI.UiContext.Engine.EditorStart(SceneManager.ActiveScene);
        
    }
}
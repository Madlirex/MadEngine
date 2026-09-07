using MadEngine.Core;
using MadEngine.Core.SceneManagement;

namespace MadEditor;

public class EnterPlaymodeCommand : IEditorCommand
{
    public void Execute(object target)
    {
        SceneSnapshotController.TakeSnapshot(SceneManager.ActiveScene);

        EditorUI.UiContext.IsPlaying = true;
        
        EditorUI.UiContext.Engine.Awake(SceneManager.ActiveScene);
        EditorUI.UiContext.Engine.Start(SceneManager.ActiveScene);
    }
}

public class ExitPlaymodeCommand : IEditorCommand
{
    public void Execute(object target)
    {
        EditorUI.UiContext.IsPlaying = false;
        
        SceneManager.ActiveScene.Destroy();
        SceneManager.LoadScene(SceneSnapshotController.RestoreSnapshot());
        
        if(EditorUI.UiContext.Selected != null)
            EditorUI.UiContext.Selected = AssetRegistry.GetObject(EditorUI.UiContext.Selected.Guid);
        
        EditorUI.UiContext.Engine.EditorStart(SceneManager.ActiveScene);
    }
}
using MadEngine.Core.SceneManagement;

namespace MadEditor.DoubleClick;

public class SceneDoubleClickCommand : DoubleClickCommand<Scene>
{
    public override void Execute(Scene target)
    {
        SceneManager.LoadScene(target);
    }
}
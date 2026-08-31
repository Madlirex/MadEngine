using MadEngine.Core.SceneManagement;

namespace MadEngine.Core;

public class AddGameObjectCommand(object target) : EngineCommand(target)
{
    public override void Execute(object target)
    {
        if (target is not GameObject targetObj) return;
        SceneManager.ActiveScene.AddObjectSafe(targetObj);
    }
}

public class DestroyGameObjectCommand(object target) : EngineCommand(target)
{
    public override void Execute(object target)
    {
        if (target is not GameObject targetObj) return;
        SceneManager.ActiveScene.DestroyObjectSafe(targetObj);
    }
}
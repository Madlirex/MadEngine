using MadEngine.Core.SceneManagement;

namespace MadEngine.Core;

public class AddGameObjectCommand(GameObject target) : EngineCommand
{
    public GameObject Target = target;
    public override void Execute()
    {
        SceneManager.ActiveScene.AddObjectSafe(Target);
    }
}

public class DestroyGameObjectCommand(GameObject target) : EngineCommand
{
    public GameObject Target = target;
    
    public override void Execute()
    {
        SceneManager.ActiveScene.DestroyObjectSafe(Target);
    }
}

public class AddComponentCommand(GameObject target, Component component) : EngineCommand
{
    public GameObject Target = target;
    public Component Component = component;
    
    public override void Execute()
    {
        Target.AddComponentSafe(Component);
    }
}

public class RemoveComponentCommand(GameObject target, Component component) : EngineCommand
{
    public GameObject Target = target;
    public Component Component = component;
    
    public override void Execute()
    {
        Target.RemoveComponentSafe(Component);
    }
}
using MadEngine.Core;
using MadEngine.Core.SceneManagement;

namespace MadEditor;

public class AddEmptyCommand : PopupCommand<GameObject>
{
    public override string Path => "Add Empty";
    public override void Execute(GameObject target)
    {
        GameObject go = new GameObject
        {
            Transform =
            {
                Parent = target.Transform
            }
        };
        
        SceneManager.ActiveScene.Add(go);
    }
}

public class AddEmptyParentCommand : PopupCommand<GameObject>
{
    public override string Path => "Add Empty Parent";
    public override void Execute(GameObject target)
    {
        GameObject go = new GameObject();

        SceneManager.ActiveScene.Add(go);

        go.Transform.Parent = target.Transform.Parent;
        target.Transform.Parent = go.Transform;
    }
}
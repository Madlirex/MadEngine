using MadEngine.Core;

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
    }
}

public class AddEmptyParentCommand : PopupCommand<GameObject>
{
    public override string Path => "Add Empty Parent";
    public override void Execute(GameObject target)
    {
        GameObject go = new GameObject()
        {
            Transform =
            {
                Parent = target.Transform.Parent
            }
        };
        target.Transform.Parent = go.Transform;
    }
}
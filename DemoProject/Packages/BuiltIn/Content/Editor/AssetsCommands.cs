using MadEngine.Core;
using MadEngine.Core.SceneManagement;

namespace MadEditor.Commands;

public class DeleteAssetCommand : PopupCommand<Asset>
{
    public override string Path => "Delete";
    public override void Execute(Asset target)
    {
        File.Delete(target!.AbsolutePath);
        target.Destroy();
    }
}
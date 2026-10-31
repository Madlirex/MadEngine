using MadEngine.Core;
using MadEngine.Core.SceneManagement;

namespace MadEditor.DoubleClick;

public class SceneDoubleClickCommand : DoubleClickCommand<Scene>
{
    public override void Execute(Scene target)
    {
        SceneManager.LoadScene(target);
    }
}

public class ImageDoubleClickCommand : DoubleClickCommand<ImageAsset>
{
    public override void Execute(ImageAsset target)
    {
        try
        {
            WindowsHelper.OpenFile(target.AbsolutePath);
        }
        catch (Exception e)
        {
            Debug.LogError($"Could not open image: {e.Message}");
        }
    }
}
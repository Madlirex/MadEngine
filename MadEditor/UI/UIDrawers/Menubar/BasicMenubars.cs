using MadEngine.Core;

namespace MadEditor;

public class SaveFileMenubar : MenubarCommand
{
    public override string Path { get; set; } = "File/Save";
    public override void Execute(EditorUIContext context)
    {
        AssetManager.SaveProject(AssetRegistry.Assets);
    }
}

public class ExitFileMenubar : MenubarCommand
{
    public override string Path { get; set; } = "File/Exit";
    public override void Execute(EditorUIContext context)
    {
        context.Window.Close();
    }
}
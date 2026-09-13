using MadEngine.Core;

namespace MadEditor;

[Order(600)]
public class SaveFileMenubar : MenubarCommand
{
    public override string Path { get; set; } = "File/Save";
    public override void Execute(EditorUIContext context)
    {
        AssetManager.SaveProject(AssetRegistry.Assets);
    }
}

[Order(1500)]
public class ExitFileMenubar : MenubarCommand
{
    public override string Path { get; set; } = "File/Exit";
    public override void Execute(EditorUIContext context)
    {
        context.Window.Close();
    }
}
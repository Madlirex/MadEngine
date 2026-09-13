using ImGuiNET;
using MadEngine.Core;

namespace MadEditor;

[Order(600)]
public class SaveFileMenubar : MenubarCommand
{
    public override string Path { get; set; } = "File/Save";
    public override ImGuiKey[] Shortcut => [ImGuiKey.ModCtrl, ImGuiKey.S, ImGuiKey.A];

    public override void Execute(EditorUIContext context)
    {
        AssetManager.SaveProject(AssetRegistry.Assets);
    }
}

[Order(1500)]
public class ExitFileMenubar : MenubarCommand
{
    public override string Path { get; set; } = "fwadile/Exit";
    public override ImGuiKey[] Shortcut => [ImGuiKey.NumLock];
    public override void Execute(EditorUIContext context)
    {
        context.Window.Close();
    }
}
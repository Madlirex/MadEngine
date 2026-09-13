using ImGuiNET;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;

[assembly: CategoryOrder("File", 20)]
[assembly: CategoryOrder("Edit", 50)]
[assembly: CategoryOrder("GameObject", 80)]
[assembly: CategoryOrder("Windows", 110)]

namespace MadEditor.Menubar;

[Order(600)]
public class SaveFileMenubar : MenubarCommand
{
    public override string Path { get; set; } = "File/Save";
    public override ImGuiKey[] Shortcut => [ImGuiKey.ModCtrl, ImGuiKey.S];

    public override void Execute(EditorUIContext context)
    {
        AssetManager.SaveProject(AssetRegistry.Assets);
    }
}

[Order(1500)]
public class ExitFileMenubar : MenubarCommand
{
    public override string Path { get; set; } = "File/Exit";
    public override ImGuiKey[] Shortcut => [ImGuiKey.ModCtrl, ImGuiKey.E];
    public override void Execute(EditorUIContext context)
    {
        context.Window.Close();
    }
}

[Order(10)]
public class UndoMenubarCommand : MenubarCommand
{
    public override string Path { get; set; } = "Edit/Undo";
    public override ImGuiKey[] Shortcut => [ImGuiKey.ModCtrl, ImGuiKey.Z];
    public override void Execute(EditorUIContext context)
    {
        Debug.Log("Undoing (not implemented yet, sorry)...");
    }
}

[Order(20)]
public class RedoMenubarCommand : MenubarCommand
{
    public override string Path { get; set; } = "Edit/Redo";
    public override ImGuiKey[] Shortcut => [ImGuiKey.ModCtrl, ImGuiKey.ModShift, ImGuiKey.Z];
    public override void Execute(EditorUIContext context)
    {
        Debug.Log("Redoing (not implemented yet, sorry)...");
    }
}

[Order(10)]
public class CreateGameObjectMenubar : MenubarCommand
{
    public override string Path { get; set; } = "GameObject/Create Empty";
    public override void Execute(EditorUIContext context)
    {
        SceneManager.ActiveScene.Add(new GameObject());
    }
}

[Order(1200)]
public class RecompileScriptsMenubar : MenubarCommand
{
    public override string Path { get; set; } = "GameObject/Recompile Scripts";
    public override ImGuiKey[] Shortcut => [ImGuiKey.C];

    public override void Execute(EditorUIContext context)
    {
        context.EnqueueCommand(new RecompilationCommand());
    }
}

public abstract class OpenPanelCommand<T> : MenubarCommand where T : PanelDrawer, new()
{
    public override string Path { get; set; } = $"Windows/{typeof(T).GetCustomName()}";

    public override void Execute(EditorUIContext context)
    {
        var panelDrawer = new T
        {
            PanelRegion = PanelRegion.Floating
        };
        PanelSystem.AddPanel(panelDrawer);
    }
}

[Order(100)]
public class OpenSceneHierarchyCommand : OpenPanelCommand<HierarchyDrawer>;

[Order(200)]
public class OpenInspectorCommand : OpenPanelCommand<InspectorPanelDrawer>;

[Order(1230)]
public class OpenSceneViewCommand : OpenPanelCommand<ViewportDrawer>;

[Order(1260)]
public class OpenGameViewCommand : OpenPanelCommand<GameDrawer>;
    
[Order(300)]
public class OpenConsoleCommand : OpenPanelCommand<ConsoleDrawer>;

[Order(400)]
public class OpenProjectCommand : OpenPanelCommand<ProjectPanelDrawer>;

[Order(500)]
public class OpenStatsCommand : OpenPanelCommand<StatsDrawer>;

[Order(2500)]
public class OpenPackageManagerCommand : OpenPanelCommand<PackageManagerDrawer>;
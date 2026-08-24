namespace MadEditor;

public class ClosePanelCommand(PanelDrawer drawer) : IEditorCommand
{
    private PanelDrawer _drawer = drawer;

    public void Execute(EditorUIContext context)
    {
        PanelSystem.DeletePanel(_drawer);
    }
}
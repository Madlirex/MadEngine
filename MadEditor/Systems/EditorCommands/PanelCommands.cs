namespace MadEditor;

public class ClosePanelCommand(PanelDrawer drawer) : IEditorCommand
{
    private PanelDrawer _drawer = drawer;

    public void Execute()
    {
        PanelSystem.DeletePanel(_drawer);
    }
}
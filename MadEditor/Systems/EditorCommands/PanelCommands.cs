namespace MadEditor;

public class ClosePanelCommand(IPanelDrawer drawer) : IEditorCommand
{
    private IPanelDrawer _drawer = drawer;

    public void Execute()
    {
        PanelSystem.DeletePanel(_drawer);
    }
}
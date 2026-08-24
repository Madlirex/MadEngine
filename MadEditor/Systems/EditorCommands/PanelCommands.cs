namespace MadEditor;

public class ClosePanelCommand(PanelDrawer drawer) : IEditorCommand
{
    private PanelDrawer _drawer = drawer;

    public void Execute(object target)
    {
        PanelSystem.DeletePanel(_drawer);
    }
}
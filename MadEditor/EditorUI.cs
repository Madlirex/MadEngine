using ImGuiNET;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;
using OpenTK.Windowing.Desktop;

namespace MadEditor;


public class EditorUI
{
    public static EditorUIContext UiContext => _uiContext;
    private static EditorUIContext _uiContext = null!;

    public EditorUI(Engine engine)
    {
        _uiContext = new EditorUIContext(engine);
    }
    
    public void Initialize()
    {
        Console.WriteLine("Intializing UI...");
        PanelSystem.Initialize();
    }

    public void Draw(GameWindow wnd)
    {
        _uiContext.Window = wnd;

        PanelSystem.Draw(_uiContext);
        PopupManager.Draw(_uiContext);
        MenubarCommandsRegistry.Draw(_uiContext);

        _uiContext.ExecuteCommands();
        _uiContext.ClearCommands();
    }
}
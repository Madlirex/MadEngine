using ImGuiNET;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;
using OpenTK.Windowing.Desktop;

namespace MadEditor;


public class EditorUI
{
    private readonly GameObject _cameraObject;
    private readonly SceneFramebuffer _sceneFbo;

    public static EditorUIContext UiContext => _uiContext;
    private static EditorUIContext _uiContext = new();

    public EditorUI(GameObject cameraObject, SceneFramebuffer sceneFbo)
    {
        _cameraObject = cameraObject;
        _sceneFbo = sceneFbo;
        
        _uiContext = new EditorUIContext()
        {
            CameraObject = _cameraObject,
            SceneFbo = _sceneFbo
        };
    }

    public void Initialize()
    {
        PanelSystem.Initialize();
    }

    public void Draw(GameWindow wnd)
    {
        _uiContext.Window = wnd;
        _uiContext.CameraObject = _cameraObject;
        _uiContext.SceneFbo = _sceneFbo;
        PanelSystem.Draw(_uiContext);
        PopupManager.Draw(_uiContext);
        
        DrawMainMenuBar();
        _uiContext.ExecuteCommands();
        _uiContext.ClearCommands();
    }

    public void DrawMainMenuBar()
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("Save", "Ctrl+N"))
                {
                    AssetManager.SaveProject(AssetRegistry.Assets);
                }

                if (ImGui.MenuItem("Open", "Ctrl+O"))
                {
                    ScriptDomain.Compile(Directory.GetFiles(AssetManager.ProjectPath, "*.cs", SearchOption.AllDirectories));
                }

                ImGui.Separator();

                if (ImGui.MenuItem("Exit"))
                {
                    Console.WriteLine("Exiting");
                }

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Edit"))
            {
                ImGui.MenuItem("Undo", "Ctrl+Z");
                ImGui.MenuItem("Redo", "Ctrl+Y");
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("GameObject"))
            {
                if (ImGui.BeginMenu("Create"))
                {
                    if (ImGui.MenuItem("Cube"))
                    {
                        
                    }
                    ImGui.EndMenu();
                }
                ImGui.MenuItem("Game");
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Windows"))
            {
                foreach (Type type in PanelSystem.PanelDrawers)
                {
                    if (ImGui.MenuItem(type.GetCustomName()))
                    {
                        if(Activator.CreateInstance(type) is not PanelDrawer panelDrawer) continue;
                        panelDrawer.PanelRegion = PanelRegion.Floating;
                        PanelSystem.AddPanel(panelDrawer);
                    }
                }
                ImGui.EndMenu();
            }
            
            ImGui.EndMainMenuBar();
        }
    }
}
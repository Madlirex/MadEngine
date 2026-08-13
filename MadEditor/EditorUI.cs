using System.Numerics;
using ImGuiNET;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace MadEditor;


public class EditorUI
{
    private readonly GameObject _cameraObject;
    private readonly SceneFramebuffer _sceneFbo;

    private EditorUIContext _uiContext;

    public EditorUI(GameObject cameraObject, SceneFramebuffer sceneFbo)
    {
        _cameraObject = cameraObject;
        _sceneFbo = sceneFbo;
        
        _uiContext = new EditorUIContext()
        {
            CameraObject = _cameraObject,
            SceneFbo = _sceneFbo
        };
        
        PanelSystem.Initialize();
        PanelSystem.CreatePanel<HierarchyDrawer>();
        PanelSystem.CreatePanel<InspectorPanelDrawer>();
        PanelSystem.CreatePanel<ViewportDrawer>();
        PanelSystem.CreatePanel<StatsDrawer>();
    }

    public void Draw(GameWindow wnd)
    {
        _uiContext.Window = wnd;
        _uiContext.CameraObject = _cameraObject;
        _uiContext.SceneFbo = _sceneFbo;
        PanelSystem.Draw(_uiContext);
        
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
                    Console.WriteLine("Opening");
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
                        GameObject obj = new GameObject();
                        Camera camera = GameObject.FindFirstComponent<Camera>()!;
                        obj.Transform.Position = camera.GameObject.Transform.Position + camera.Front * 2;

                        Texture diffuse = new Texture("Textures/container2.png");
                        Texture specular = new Texture("Textures/container2_specular.png");
                        SceneManager.ActiveScene.Add(obj);
                    }
                    ImGui.EndMenu();
                }
                ImGui.MenuItem("Game");
                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Windows"))
            {
                if (ImGui.MenuItem("Package Manager"))
                {
                    PanelSystem.AddPanel(new PackageManagerDrawer() {PanelRegion = PanelRegion.Floating});
                }
                ImGui.EndMenu();
            }
            
            ImGui.EndMainMenuBar();
        }
    }
}
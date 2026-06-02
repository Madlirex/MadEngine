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

    private GameObject? _selected;
    private Vector2 _viewportSize;
    
    private const float LeftPanelWidth   = 0.18f;
    private const float RightPanelWidth  = 0.22f;
    private const float BottomPanelHeight = 0.20f;

    private static readonly ImGuiWindowFlags FixedPanel =
        ImGuiWindowFlags.NoMove        |
        ImGuiWindowFlags.NoResize      |
        ImGuiWindowFlags.NoCollapse    |
        ImGuiWindowFlags.NoBringToFrontOnFocus;

    public EditorUI(GameObject cameraObject, SceneFramebuffer sceneFbo)
    {
        _cameraObject = cameraObject;
        _sceneFbo = sceneFbo;
    }

    public void Draw(GameWindow wnd)
    {
        var screenPos = ImGui.GetMainViewport().WorkPos;
        var screenSize = ImGui.GetMainViewport().WorkSize;

        float leftW = MathF.Floor(screenSize.X * LeftPanelWidth);
        float rightW = MathF.Floor(screenSize.X * RightPanelWidth);
        float bottomH = MathF.Floor(screenSize.Y * BottomPanelHeight);
        float centerW = screenSize.X - leftW - rightW;
        float topH = screenSize.Y - bottomH;
        
        ImGui.SetNextWindowPos(screenPos);
        ImGui.SetNextWindowSize(new Vector2(leftW, screenSize.Y));
        ImGui.Begin("Hierarchy", FixedPanel);
        DrawHierarchy();
        ImGui.End();
        
        ImGui.SetNextWindowPos(new Vector2(screenPos.X + leftW + centerW, screenPos.Y));
        ImGui.SetNextWindowSize(new Vector2(rightW, screenSize.Y));
        ImGui.Begin("Inspector", FixedPanel);
        InspectorDrawer.Draw(_selected);
        ImGui.End();
        
        ImGui.SetNextWindowPos(new Vector2(screenPos.X + leftW, screenPos.Y));
        ImGui.SetNextWindowSize(new Vector2(centerW, topH));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
        ImGui.Begin("Scene Viewport", FixedPanel);
        ImGui.PopStyleVar();
        DrawViewport(wnd, centerW, topH);
        ImGui.End();

        ImGui.SetNextWindowPos(new Vector2(screenPos.X + leftW, screenPos.Y + topH));
        ImGui.SetNextWindowSize(new Vector2(centerW, bottomH));
        ImGui.Begin("Stats", FixedPanel);
        DrawStats(wnd);
        
        DrawMainMenuBar();
        
        ImGui.End();
    }

    public void DrawMainMenuBar()
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("File"))
            {
                if (ImGui.MenuItem("New", "Ctrl+N"))
                {
                    Console.WriteLine("Newing");
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

                        MeshRenderer meshRenderer = new MeshRenderer(new Mesh(Tests.Vertices, Tests.Indices),
                            new Material(ShaderSystem.LitShader, diffuse, specular));
                        obj.AddComponent(meshRenderer);
                        SceneManager.ActiveScene.Add(obj);
                    }
                    ImGui.EndMenu();
                }
                ImGui.MenuItem("Game");
                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }
    }
    private void DrawHierarchy()
    {
        var objects = SceneManager.ActiveScene.GameObjects;
        foreach (var go in objects)
        {
            string label = go.Name + "##" + go.Id;
            bool sel = go == _selected;
            if (ImGui.Selectable(label, sel))
                _selected = go;
        }
    }

    private void DrawViewport(GameWindow wnd, float panelW, float panelH)
    {
        float titleBarH  = ImGui.GetFrameHeight();
        float availableW = panelW;
        float availableH = panelH - titleBarH;

        if (availableW > 1 && availableH > 1)
        {
            var newSize = new Vector2(availableW, availableH);
            if (newSize.X != _viewportSize.X || newSize.Y != _viewportSize.Y)
            {
                _viewportSize = newSize;
                _sceneFbo.Resize((int)availableW, (int)availableH);

                Camera cam = _cameraObject.GetComponent<Camera>()!;
                cam.Width = (int)availableW;
                cam.Height = (int)availableH;
            }
        }
        
        if (ImGui.IsWindowHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            wnd.CursorState = CursorState.Grabbed;
        
        ImGui.Image(_sceneFbo.ColorTexture, _viewportSize, new Vector2(0, 1), new Vector2(1, 0));

        if (wnd.CursorState == CursorState.Normal)
        {
            ImGui.SetCursorPos(new Vector2(8, ImGui.GetFrameHeight() + 4));
            ImGui.TextDisabled("Right-click + WASD to fly  |  Esc to release");
        }
    }

    private void DrawStats(GameWindow wnd)
    {
        ImGui.Text($"FPS        : {1.0 / wnd.UpdateTime:F0}");
        ImGui.Text($"Frame time : {wnd.UpdateTime * 1000.0:F2} ms");

        var pos = _cameraObject.Transform.Position;
        ImGui.Text($"Camera     : {pos.X:F2}, {pos.Y:F2}, {pos.Z:F2}");
        ImGui.Text($"Viewport   : {_viewportSize.X:F0} x {_viewportSize.Y:F0}");
    }
}
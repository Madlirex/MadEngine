using ImGuiNET;
using MadEditor.PackageManagement;
using MadEngine.Core;
using MadEngine.Core.SceneManagement;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;


namespace MadEditor;

public class EditorWindow : GameWindow
{
    public Engine Engine;

    private Vector2 _lastPos;
    private bool _firstMove = true;

    private ImGuiController _imGui;
    private EditorUI _editorUI;
    private SceneGridRenderer _gridRenderer;

    private DateTime _start;
    
    public EditorWindow(int width, int height, string title) : base(new GameWindowSettings()
    {
        UpdateFrequency = 60
    },
        new NativeWindowSettings()
    {
        ClientSize = (width, height), Title = title

    })
    {
        _start = DateTime.Now;
        
        Application.Directory = AssetManager.ProjectPath;
        
        Engine = new Engine();
        
        CursorState = CursorState.Normal;
        _imGui = new ImGuiController(width, height);
        _editorUI = new EditorUI(Engine);
        _gridRenderer = new SceneGridRenderer();
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        Engine.Initialize();
        Console.WriteLine("Compiling");
        AssetManager.RecompileScripts(false);
        
        RegistryBootstrapper.InitializeAll();
        
        PackageManager.LoadPackageMetas();

        Console.WriteLine("loading package");
        PackageManager.LoadPackages();
        Console.WriteLine("loading assets");
        AssetManager.LoadProject();
        _editorUI.Initialize();

        Console.WriteLine("Loading scene");
        SceneManager.LoadScene(0);

        DateTime end = DateTime.Now;
        Debug.Log($"Startup finished in: {end - _start}");
        
        Engine.EditorStart(SceneManager.ActiveScene);
    }

    protected override void OnTextInput(TextInputEventArgs e)
    {
        base.OnTextInput(e);
        _imGui.PressChar((char)e.Unicode);
    }

    protected override void OnFocusedChanged(FocusedChangedEventArgs e)
    {
        base.OnFocusedChanged(e);
        
        CursorState = IsFocused ? CursorState.Normal : CursorState.Normal;
    }

    protected override void OnUnload()
    {
        base.OnUnload();
        PackageManager.SavePackageMetas();

        CursorState = CursorState.Normal;
        
        Engine.Dispose();
        foreach (var vp in EditorUI.UiContext.GetAllViewports())
        {
            vp.Dispose();
        }
        _imGui.Dispose();    
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        
        foreach (var vp in EditorUI.UiContext.GetAllViewports())
        {
            if (vp.Size.X <= 1 || vp.Size.Y <= 1) continue;
        
            vp.Framebuffer.Bind();
            GL.Viewport(0, 0, (int)vp.Size.X, (int)vp.Size.Y);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
            Camera camera = vp.CameraComponent;
        
            Engine.Render(SceneManager.ActiveScene, camera);
        
            _gridRenderer.Render(
                camera.GetViewMatrix(), 
                camera.GetPerspectiveMatrix(), 
                vp.CameraObject.Transform.Position, 
                camera.DepthFar
            );
        }
        
        SceneFramebuffer.Unbind();
        
        GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        _editorUI.Draw(this);
        _imGui.Render();
    
        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        _imGui.Update(this, (float)args.Time);
        
        UpdateCamera(args);
        if(EditorUI.UiContext.IsPlaying)
        {
            Engine.Update((float)args.Time, SceneManager.ActiveScene);
        }
        else
        {
            Engine.EditorUpdate((float)args.Time, SceneManager.ActiveScene);
        }
    }
    
    public void UpdateCamera(FrameEventArgs args)
    {
        GameObject? currentFlyingCamera = EditorUI.UiContext.ActiveViewport?.CameraObject;
        if (currentFlyingCamera == null || !IsFocused) return;
    
        if (!IsFocused)
        {
            return;
        }

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            CursorState = CursorState.Normal;
        }
        
        if (CursorState != CursorState.Grabbed)
        {
            _firstMove = true;
            return;
        }

        Camera camera = EditorUI.UiContext.ActiveViewport!.CameraComponent;
        float speed = camera.Speed * (float)args.Time;
        KeyboardState input = KeyboardState;

        if (input.IsKeyDown(Keys.LeftControl))
        {
            speed *= 5;
        }
        
        if (input.IsKeyDown(Keys.W))
        {
            currentFlyingCamera.Transform.Position += camera.Front * speed;
            //SceneManager.ActiveScene.Add(new GameObject());
        }

        if (input.IsKeyDown(Keys.S))
        {
            currentFlyingCamera.Transform.Position -= camera.Front * speed;
        }

        if (input.IsKeyDown(Keys.A))
        {
            currentFlyingCamera.Transform.Position -= Vector3.Normalize(Vector3.Cross(camera.Front, camera.Up)) * speed; 
        }

        if (input.IsKeyDown(Keys.D))
        {
            currentFlyingCamera.Transform.Position += Vector3.Normalize(Vector3.Cross(camera.Front, camera.Up)) * speed;
        }

        if (input.IsKeyDown(Keys.Space))
        {
            currentFlyingCamera.Transform.Position += camera.Up * speed;
        }

        if (input.IsKeyDown(Keys.LeftShift))
        {
            currentFlyingCamera.Transform.Position -= camera.Up * speed;
        }
        
        const float sensitivity = 0.2f;
        
        MouseState mouse = MouseState;
        if (_firstMove)
        {
            _lastPos = new Vector2(mouse.X, mouse.Y);
            _firstMove = false;
        }
        else
        {
            float deltaX = mouse.X - _lastPos.X;
            float deltaY = mouse.Y - _lastPos.Y;
            _lastPos = new Vector2(mouse.X, mouse.Y);
            
            camera.Yaw += deltaX * sensitivity;
            camera.Pitch -= deltaY * sensitivity;
        }
    }
    
    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        if (CursorState == CursorState.Grabbed)
            EditorUI.UiContext.ActiveViewport!.CameraComponent.Fov -= e.OffsetY;
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);
        
        _imGui.Resize(e.Width, e.Height);
    }
}
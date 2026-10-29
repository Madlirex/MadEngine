using MadEngine.Core;
using MadEngine.Core.SceneManagement;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;


namespace MadEngine.Runtime;

public class RuntimeWindow : GameWindow
{
    private Engine _engine;
    private Camera Camera => Camera.MainCamera ?? GameObject.FindFirstComponent<Camera>();

    public static RuntimeWindow Create(int width, int height, string title)
    {
        var gameSettings = new GameWindowSettings() { UpdateFrequency = 60 };

        var windowSettings = new NativeWindowSettings()
        {
            ClientSize = (width, height),
            Title = title,
            APIVersion = new Version(3, 3),
            Profile = ContextProfile.Core
        };
        
        try
        {
            return new RuntimeWindow(gameSettings, windowSettings);
        }
        catch (GLFWException e)
        {
            Debug.LogError($"GLFW Exception: {e.Message}");
            windowSettings.Profile = ContextProfile.Any;
            return new RuntimeWindow(gameSettings, windowSettings);
        }
    }
    
    public RuntimeWindow(GameWindowSettings gameSettings, NativeWindowSettings windowSettings) : base(gameSettings, windowSettings)
    {
        GameObject obj = new GameObject();
        
        var cam = (Camera?)obj.AddComponentUnsafe(new Camera());
        
        cam!.IsMainCamera = true;
        cam.Width = ClientSize.X;
        cam.Height = ClientSize.Y;
        
        SceneManager.ActiveScene.AddObjectUnsafe(obj);
        _engine = new Engine();
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        
        _engine.Initialize();
        
        _engine.Awake(SceneManager.ActiveScene);
        _engine.Start(SceneManager.ActiveScene);
    }

    protected override void OnFocusedChanged(FocusedChangedEventArgs e)
    {
        base.OnFocusedChanged(e);
        
        CursorState = IsFocused ? CursorState.Grabbed : CursorState.Normal; // editor always normal
    }

    protected override void OnUnload()
    {
        base.OnUnload();

        CursorState = CursorState.Normal;
        _engine.Dispose();
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        
        _engine.Render(SceneManager.ActiveScene, Camera);

        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);
        
        _engine.Update((float)args.Time, SceneManager.ActiveScene);
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);
        Camera.Width = e.Width;
        Camera.Height = e.Height;
        GL.Viewport(0, 0, e.Width, e.Height);
    }
}
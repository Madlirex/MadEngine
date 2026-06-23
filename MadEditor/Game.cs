using ImGuiNET;
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
    private Engine _engine;
    
    private GameObject _camera;

    private Vector2 _lastPos;
    private bool _firstMove = true;

    private ImGuiController _imGui;
    private SceneFramebuffer _sceneFbo;
    private EditorUI _editorUI;
    
    public EditorWindow(int width, int height, string title) : base(new GameWindowSettings()
    {
        UpdateFrequency = 60
    },
        new NativeWindowSettings()
    {
        ClientSize = (width, height), Title = title

    })
    {
        _engine = new Engine();
        ShaderSystem.InitializeLitShader("Shaders/lit.vert", "Shaders/lit.frag");
        ShaderSystem.InitializeUnlitShader("Shaders/unlit.vert", "Shaders/unlit.frag");
        
        FieldDrawerRegistry.Initialize();

        _camera = new GameObject();
        _camera.Name = "MainCamera";
        _camera.AddComponent(new Camera());
        
        CursorState = CursorState.Normal;
        _camera.GetComponent<Camera>()!.Width = width;
        _camera.GetComponent<Camera>()!.Height = height;
        
        MeshRenderer defaultRenderer = new MeshRenderer
        {
            Material = new Material(ShaderSystem.LitShader, diffuseColor: new Vector4(1f, 1f, 1f, 1f), specularColor: new Vector4(0.5f, 0.5f, 0.5f, 1f))
        };

        Transform defaultTransform = new Transform()
        {
            Position = new Vector3(0f, 0f, 0f),
        };

        MeshRenderer lampRenderer = new MeshRenderer()
        {
            Mesh = new Mesh([], []),
            Material = new Material(ShaderSystem.UnlitShader, null, null, Vector4.One, Vector4.One, Vector4.One, 0f)
        };
        
        GameObject light = new GameObject();
        light.Name = "Light";
        light.Transform.Position = new Vector3(-4f, 4f, 0f);
        light.Transform.Rotation = new Quaternion(new Vector3(-1, 1, 0f) * 180);
        Light lightComp = new DirectionalLight()
        {
            Direction = new Vector3(1f, -1.5f, 1f),
        };
        light.AddComponent(lightComp);
        GameObject cube = new GameObject();
        cube.Name = "Cube";
        cube.AddComponent(defaultRenderer);
        
        Scene scene = new Scene();
        scene.Add(light);
        scene.Add(cube);
        SceneManager.ActiveScene = scene;

        _imGui = new ImGuiController(width, height);
        _sceneFbo = new SceneFramebuffer(width, height);
        _editorUI = new EditorUI(_camera, _sceneFbo);
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        
        _engine.Initialize();

        AssetManager.RecompileScripts();
        
        _engine.EditorStart(SceneManager.ActiveScene);
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

        CursorState = CursorState.Normal;
        
        _engine.Dispose();
        _sceneFbo.Dispose(); 
        _imGui.Dispose();    
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        
        _sceneFbo.Bind();

        Camera camera = _camera.GetComponent<Camera>()!;

        _engine.Render(SceneManager.ActiveScene, camera);
        
        SceneFramebuffer.Unbind();
        GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        _editorUI.Draw(this);
        _imGui.Render();

        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        _imGui.Update(this, (float)args.Time);
        
        UpdateCamera(args);
        _engine.EditorUpdate((float)args.Time, SceneManager.ActiveScene);
    }
    
    public void UpdateCamera(FrameEventArgs args)
    {
        if (!IsFocused)
        {
            return;
        }
        
        if (CursorState != CursorState.Grabbed)
        {
            _firstMove = true;
            return;
        }

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            CursorState = CursorState.Normal;
        }

        Camera camera = _camera.GetComponent<Camera>()!;
        KeyboardState input = KeyboardState;
        float speed = camera.Speed * (float)args.Time;

        if (input.IsKeyDown(Keys.LeftControl))
        {
            speed *= 5;
        }
        
        if (input.IsKeyDown(Keys.W))
        {
            _camera.Transform.Position += camera.Front * speed;
        }

        if (input.IsKeyDown(Keys.S))
        {
            _camera.Transform.Position -= camera.Front * speed;
        }

        if (input.IsKeyDown(Keys.A))
        {
            _camera.Transform.Position -= Vector3.Normalize(Vector3.Cross(camera.Front, camera.Up)) * speed; 
        }

        if (input.IsKeyDown(Keys.D))
        {
            _camera.Transform.Position += Vector3.Normalize(Vector3.Cross(camera.Front, camera.Up)) * speed;
        }

        if (input.IsKeyDown(Keys.Space))
        {
            _camera.Transform.Position += camera.Up * speed;
        }

        if (input.IsKeyDown(Keys.LeftShift))
        {
            _camera.Transform.Position -= camera.Up * speed;
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
            _camera.GetComponent<Camera>()!.Fov -= e.OffsetY;
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);
        _camera.GetComponent<Camera>()!.Width = e.Width;
        _camera.GetComponent<Camera>()!.Height = e.Height;
        GL.Viewport(0, 0, e.Width, e.Height);
        _imGui.Resize(e.Width, e.Height);
    }
}
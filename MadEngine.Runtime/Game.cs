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
    
    private GameObject _camera;
    private GameObject _light;

    private Vector2 _lastPos;
    private bool _firstMove = true;
    
    public RuntimeWindow(int width, int height, string title) : base(new GameWindowSettings()
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

        _camera = new GameObject(new Transform());
        _camera.Name = "MainCamera";
        _camera.AddComponent(new Camera());
        
        CursorState = CursorState.Normal; // changed: editor uses normal cursor by default
        _camera.GetComponent<Camera>()!.Width = width;
        _camera.GetComponent<Camera>()!.Height = height;

        MeshRenderer defaultRenderer = new MeshRenderer(new Mesh(Tests.Vertices, Tests.Indices), 
            new Material(ShaderSystem.LitShader, new Texture("Textures/container2.png"),
                new Texture("Textures/container2_specular.png")));
        Transform defaultTransform = new Transform()
        {
            Position = new Vector3(0f, 0f, 0f),
        };

        MeshRenderer lampRenderer = new MeshRenderer(new Mesh([], []),
            new Material(ShaderSystem.UnlitShader, null, null, Vector4.One, Vector4.One, Vector4.One, 0f));
        Transform lampTransform = new Transform()
        {
            Position = new Vector3(-4f, 4f, 0f),
            Rotation = new Vector3(-1, 1, 0f) * 180
        };
        _light = new GameObject(lampTransform);
        _light.Name = "Light";
        Light light = new SpotLight
        {
            Direction = new Vector3(1f, -1f, 0f),
            CutOff = 30f,
            OuterCutOff = 35f
        };
        _light.AddComponent(light);
        GameObject cube = new GameObject(defaultTransform);
        cube.Name = "Cube";
        cube.AddComponent(defaultRenderer);
        
        Scene scene = new Scene();
        scene.Add(_camera);
        scene.Add(_light);
        scene.Add(cube);
        SceneManager.ActiveScene = scene;
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

        Camera camera = _camera.GetComponent<Camera>()!;
        
        _light.Transform.Position = _camera.Transform.Position;
        _light.GetComponent<SpotLight>()!.Direction = camera.Front;

        _engine.Render(SceneManager.ActiveScene, camera);

        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        if (!IsFocused)
        {
            return;
        }

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
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
            _camera.Transform.Position += camera.Front * speed; //Forward 
        }

        if (input.IsKeyDown(Keys.S))
        {
            _camera.Transform.Position -= camera.Front * speed; //Backwards
        }

        if (input.IsKeyDown(Keys.A))
        {
            _camera.Transform.Position -= Vector3.Normalize(Vector3.Cross(camera.Front, camera.Up)) * speed; //Left
        }

        if (input.IsKeyDown(Keys.D))
        {
            _camera.Transform.Position += Vector3.Normalize(Vector3.Cross(camera.Front, camera.Up)) * speed; //Right
        }

        if (input.IsKeyDown(Keys.Space))
        {
            _camera.Transform.Position += camera.Up * speed; //Up 
        }

        if (input.IsKeyDown(Keys.LeftShift))
        {
            _camera.Transform.Position -= camera.Up * speed; //Down
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
        _engine.Update((float)args.Time, SceneManager.ActiveScene);
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
    }
}
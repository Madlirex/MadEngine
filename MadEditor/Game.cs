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
    
    private Shader _shader;
    private Shader _lampShader;
    private GameObject _camera;
    private GameObject _light;

    private Vector2 _lastPos;
    private double _deltaTime;
    private double _time;
    private bool _firstMove = true;

    // ── Editor additions ──────────────────────────────────────────────────────
    private ImGuiController _imGui;
    private SceneFramebuffer _sceneFbo;
    private EditorUI _editorUI;
    // ─────────────────────────────────────────────────────────────────────────
    
    public RuntimeWindow(int width, int height, string title) : base(new GameWindowSettings()
    {
        UpdateFrequency = 60
    },
        new NativeWindowSettings()
    {
        ClientSize = (width, height), Title = title

    })
    {
        _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
        _lampShader = new Shader("Shaders/shader.vert", "Shaders/lamp.frag");

        _camera = new GameObject(new Transform());
        _camera.Name = "MainCamera";
        _camera.AddComponent(new Camera());
        
        CursorState = CursorState.Normal; // changed: editor uses normal cursor by default
        _camera.GetComponent<Camera>()!.Width = width;
        _camera.GetComponent<Camera>()!.Height = height;

        MeshRenderer defaultRenderer = new MeshRenderer(new Mesh(Tests.Vertices, Tests.Indices), 
            new Material(_shader, new Texture("Textures/container2.png"),
                new Texture("Textures/container2_specular.png")));
        Transform defaultTransform = new Transform()
        {
            Position = new Vector3(0f, 0f, 0f),
        };

        MeshRenderer lampRenderer = new MeshRenderer(new Mesh([], []),
            new Material(_lampShader, null, null, Vector4.One, Vector4.One, Vector4.One, 0f));
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
        GL.Enable(EnableCap.DepthTest);

        // ── Editor additions ──────────────────────────────────────────────────
        _imGui = new ImGuiController(width, height);
        _sceneFbo = new SceneFramebuffer(width, height);
        _editorUI = new EditorUI(_camera, _sceneFbo);
        // ─────────────────────────────────────────────────────────────────────
    }

    protected override void OnLoad()
    {
        base.OnLoad();
        
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1f);
    }

    protected override void OnTextInput(TextInputEventArgs e) // added for ImGui text input
    {
        base.OnTextInput(e);
        _imGui.PressChar((char)e.Unicode);
    }

    protected override void OnFocusedChanged(FocusedChangedEventArgs e)
    {
        base.OnFocusedChanged(e);
        
        CursorState = IsFocused ? CursorState.Normal : CursorState.Normal; // editor always normal
    }

    protected override void OnUnload()
    {
        base.OnUnload();

        CursorState = CursorState.Normal;
        
        _shader.Dispose();
        _sceneFbo.Dispose(); // added
        _imGui.Dispose();    // added
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        _deltaTime = UpdateTime;
        base.OnRenderFrame(args);

        // ── Render scene into FBO instead of directly to screen ───────────────
        _sceneFbo.Bind();
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.Enable(EnableCap.DepthTest);
        // ─────────────────────────────────────────────────────────────────────
        
        float radius = 5f;
        float speed = 1f;

        _time += args.Time;
        float angle = (float)_time; // or accumulate your own timer

        Camera camera = _camera.GetComponent<Camera>()!;
        
        _light.Transform.Position = _camera.Transform.Position;
        _light.GetComponent<SpotLight>()!.Direction = camera.Front;

        Light.UseLights(_shader, SceneManager.ActiveScene.Lights.ToArray());
        
        Matrix4 view = camera.GetViewMatrix();
        Matrix4 projection = camera.GetPerspectiveMatrix();

        foreach (MeshRenderer meshRenderer in SceneManager.ActiveScene.MeshRenderers)
        {
            meshRenderer.Draw(view, projection);
        }

        // ── Draw ImGui editor UI on top ───────────────────────────────────────
        SceneFramebuffer.Unbind();
        GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        _editorUI.Draw(this);
        _imGui.Render();
        // ─────────────────────────────────────────────────────────────────────

        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        _imGui.Update(this, (float)args.Time); // added

        if (!IsFocused)
        {
            return;
        }

        // Only move camera when viewport is grabbed (right-click held in viewport)
        if (CursorState != CursorState.Grabbed) // added guard
        {
            _firstMove = true;
            return;
        }

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            CursorState = CursorState.Normal; // changed: release cursor instead of closing
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
    }
    
    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        if (CursorState == CursorState.Grabbed) // added guard
            _camera.GetComponent<Camera>()!.Fov -= e.OffsetY;
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);
        _camera.GetComponent<Camera>()!.Width = e.Width;
        _camera.GetComponent<Camera>()!.Height = e.Height;
        GL.Viewport(0, 0, e.Width, e.Height);
        _imGui.Resize(e.Width, e.Height); // added
    }
}
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;


namespace MadEngine;

public class Game : GameWindow
{
    private GameObject[] _scene;
    private Shader _shader;
    private Shader _lampShader;
    private Camera _camera = new();
    private Light _light;

    public static Vector4 LightColor;
    public static Vector3 LightPos;

    private Vector2 _lastPos;
    private double _deltaTime;
    private double _time;
    private bool _firstMove = true;
    
    public Game(int width, int height, string title) : base(new GameWindowSettings()
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

        CursorState = CursorState.Grabbed;
        _camera.Width = width;
        _camera.Height = height;

        MeshRenderer defaultRenderer = new MeshRenderer(new Mesh(Tests.Vertices, Tests.Indices),
            new Material(_shader, new Texture("Textures/container2.png"),
                new Texture("Textures/container2_specular.png")));
        Transform defaultTransform = new Transform()
        {
            Position = new Vector3(0f, 0f, 0f)
        };

        MeshRenderer lampRenderer = new MeshRenderer(new Mesh(Tests.Vertices, Tests.Indices),
            new Material(_lampShader, null, null, Vector4.One, Vector4.One, Vector4.One, 0f));
        Transform lampTransform = new Transform()
        {
            Position = new Vector3(2f, 0f, 0f)
        };
        _light = new Light(lampRenderer, lampTransform); 
        
        _scene = [_light, new GameObject(defaultRenderer, defaultTransform)];
        GL.Enable(EnableCap.DepthTest);
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        foreach (GameObject gameObject in _scene)
        {
            gameObject.MeshRenderer.Mesh.Initialize();
        }
        
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1f);
    }

    protected override void OnFocusedChanged(FocusedChangedEventArgs e)
    {
        base.OnFocusedChanged(e);
        
        CursorState = IsFocused ? CursorState.Grabbed : CursorState.Normal;
    }

    protected override void OnUnload()
    {
        base.OnUnload();

        CursorState = CursorState.Normal;
        
        _shader.Dispose();
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        _deltaTime = UpdateTime;
        base.OnRenderFrame(args);
        
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        float radius = 5f;
        float speed = 1f;

        _time += args.Time;
        float angle = (float)_time; // or accumulate your own timer

        _light.Transform.Position = new Vector3(
            MathF.Cos(angle * speed) * radius,
            3f,
            MathF.Sin(angle * speed) * radius
        );
        
        _shader.SetVector3("viewPos", _camera.Transform.Position);
        _shader.SetVector3("light.position", _light.Transform.Position);
        _shader.SetVector4("light.ambient", _light.AmbientColor);
        _shader.SetVector4("light.diffuse", _light.DiffuseColor);
        _shader.SetVector4("light.specular", _light.SpecularColor);
        
        Matrix4 view = _camera.GetViewMatrix();
        Matrix4 projection = _camera.GetPerspectiveMatrix();
        
        foreach (var gameObject in _scene)
        {
            //gameObject.Transform.Rotation.Y += 60 * (float)_deltaTime;
            gameObject.MeshRenderer.Draw(view, projection);
        }
        
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

        KeyboardState input = KeyboardState;
        float speed = _camera.Speed * (float)args.Time;

        if (input.IsKeyDown(Keys.LeftControl))
        {
            speed *= 5;
        }
        
        if (input.IsKeyDown(Keys.W))
        {
            _camera.Transform.Position += _camera.Front * speed; //Forward 
        }

        if (input.IsKeyDown(Keys.S))
        {
            _camera.Transform.Position -= _camera.Front * speed; //Backwards
        }

        if (input.IsKeyDown(Keys.A))
        {
            _camera.Transform.Position -= Vector3.Normalize(Vector3.Cross(_camera.Front, _camera.Up)) * speed; //Left
        }

        if (input.IsKeyDown(Keys.D))
        {
            _camera.Transform.Position += Vector3.Normalize(Vector3.Cross(_camera.Front, _camera.Up)) * speed; //Right
        }

        if (input.IsKeyDown(Keys.Space))
        {
            _camera.Transform.Position += _camera.Up * speed; //Up 
        }

        if (input.IsKeyDown(Keys.LeftShift))
        {
            _camera.Transform.Position -= _camera.Up * speed; //Down
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
            
            _camera.Yaw += deltaX * sensitivity;
            _camera.Pitch -= deltaY * sensitivity;
        }
    }
    
    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        base.OnMouseWheel(e);

        _camera.Fov -= e.OffsetY;
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);
        _camera.Width = e.Width;
        _camera.Height = e.Height;
        GL.Viewport(0, 0, e.Width, e.Height);
    }
}
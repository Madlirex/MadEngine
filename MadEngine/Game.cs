using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using Vector4 = System.Numerics.Vector4;


namespace MadEngine;

public class Game : GameWindow
{
    private GameObject[] _scene;
    private Shader _shader;
    private Shader _lampShader;
    private Camera _camera = new();
    private GameObject _light;

    public static Vector4 LightColor;
    public static Vector3 LightPos;

    private Vector2 _lastPos;
    private double _deltaTime;
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
        
        float[] vertices = {
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f, 0.0f, 0.0f, -1.0f,
            0.5f, -0.5f, -0.5f,  1.0f, 0.0f, 0.0f, 0.0f, -1.0f,
            0.5f,  0.5f, -0.5f,  1.0f, 1.0f, 0.0f, 0.0f, -1.0f,
            0.5f,  0.5f, -0.5f,  1.0f, 1.0f, 0.0f, 0.0f, -1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f, 0.0f, 0.0f, -1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f, 0.0f, 0.0f, -1.0f,

            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f, 0.0f, 0.0f, 1.0f,
            0.5f, -0.5f,  0.5f,  1.0f, 0.0f, 0.0f, 0.0f, 1.0f,
            0.5f,  0.5f,  0.5f,  1.0f, 1.0f, 0.0f, 0.0f, 1.0f,
            0.5f,  0.5f,  0.5f,  1.0f, 1.0f, 0.0f, 0.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 1.0f, 0.0f, 0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f, 0.0f, 0.0f, 1.0f,

            -0.5f,  0.5f,  0.5f,  1.0f, 0.0f, -1.0f, 0.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  1.0f, 1.0f, -1.0f, 0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f, -1.0f, 0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f, -1.0f, 0.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f, -1.0f, 0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  1.0f, 0.0f, -1.0f, 0.0f, 0.0f,

            0.5f,  0.5f,  0.5f,  1.0f, 0.0f, 1.0f, 0.0f, 0.0f,
            0.5f,  0.5f, -0.5f,  1.0f, 1.0f, 1.0f, 0.0f, 0.0f,
            0.5f, -0.5f, -0.5f,  0.0f, 1.0f, 1.0f, 0.0f, 0.0f,
            0.5f, -0.5f, -0.5f,  0.0f, 1.0f, 1.0f, 0.0f, 0.0f,
            0.5f, -0.5f,  0.5f,  0.0f, 0.0f, 1.0f, 0.0f, 0.0f,
            0.5f,  0.5f,  0.5f,  1.0f, 0.0f, 1.0f, 0.0f, 0.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f, 0.0f, -1.0f, 0.0f,
            0.5f, -0.5f, -0.5f,  1.0f, 1.0f, 0.0f, -1.0f, 0.0f,
            0.5f, -0.5f,  0.5f,  1.0f, 0.0f, 0.0f, -1.0f, 0.0f,
            0.5f, -0.5f,  0.5f,  1.0f, 0.0f, 0.0f, -1.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f, 0.0f, -1.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f, 0.0f, -1.0f, 0.0f,

            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f, 0.0f, 1.0f, 0.0f,
            0.5f,  0.5f, -0.5f,  1.0f, 1.0f, 0.0f, 1.0f, 0.0f,
            0.5f,  0.5f,  0.5f,  1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
            0.5f,  0.5f,  0.5f,  1.0f, 0.0f, 0.0f, 1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 0.0f, 0.0f, 1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f, 0.0f, 1.0f, 0.0f
        };
        
        uint[] indices =
        {
            // Front (-Z)
            0, 2, 1,
            3, 5, 4,

            // Back (+Z)
            6, 7, 8,
            9, 10, 11,

            // Left (-X)
            12,13,14,
            15,16,17,

            // Right (+X)
            18,20,19,
            21,23,22,

            // Bottom (-Y)
            24,25,26,
            27,28,29,

            // Top (+Y)
            30,32,31,
            33,35,34
        };
        
        Vector4 color = new Vector4(1f, 1f, 1f, 1f);
        Mesh mesh = new Mesh(vertices, indices);
        
        MeshRenderer meshRenderer1 = new MeshRenderer(mesh, new Material(_shader, new Texture("Textures/simkovicova.jpg"), color));
        MeshRenderer meshRenderer2 = new MeshRenderer(mesh, new Material(_shader, new Texture("Textures/charlie.jpg"), color));
        MeshRenderer meshRenderer3 = new MeshRenderer(mesh, new Material(_shader, new Texture("Textures/house.jpg"), color));
        MeshRenderer meshRenderer4 = new MeshRenderer(mesh, new Material(_shader, new Texture("Textures/matovic.jpg"), color));
        MeshRenderer meshRenderer5 = new MeshRenderer(mesh, new Material(_shader, new Texture("Textures/romana.jpg"), color));

        MeshRenderer meshRendererNormal = new MeshRenderer(mesh,
            new Material(_shader, null, new Vector4(0f, 1f, 0f, 1f)));
        
        Vector4 lightColor = new Vector4(1f, 1f, 1f, 1f);
        MeshRenderer lampRenderer = new MeshRenderer(mesh, new Material(_lampShader, null, lightColor));
        
        Transform transform1 = new()
        {
            Position = new Vector3(1.5f, 0f, -1.5f),
            Rotation = new Vector3(90f, 45f, 0f),
            Scale = new Vector3(10f, 10f, 3f)
        };
        
        Transform transform2 = new()
        {
            Position = new Vector3(-1.5f, 0f, -1.5f),
            //Rotation = new Vector3(90f, 45f, 0f)
        };
        
        Transform transform3 = new()
        {
            Position = new Vector3(1.5f, 0f, 1.5f),
            Rotation = new Vector3(90f, 45f, 0f)
        };
        Transform transform4 = new()
        {
            Position = new Vector3(-1.5f, 0f, 1.5f),
            Rotation = new Vector3(90f, 45f, 0f)
        };
        Transform transform5 = new()
        {
            Position = new Vector3(1.5f, 0f, -1.5f),
            Rotation = new Vector3(90f, 45f, 0f),
            Scale = new Vector3(90f, 90f, 50f)
        };

        Transform transformNormal = new()
        {
            Position = new Vector3(-3.5f, -1f, -1.5f),
            Rotation = new Vector3(0f, 0f, 0f),
            Scale = new Vector3(1f, 1f, 1f)
        };
        
        LightColor = lightColor;
        LightPos = transform2.Position;

        _light = new GameObject(lampRenderer, transform2);
        _shader.SetVector3("lightPos", _light.Transform.Position);
        
        _scene = [_light, new GameObject(meshRendererNormal, transformNormal)];
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
        
        _shader.SetVector3("viewPos", _camera.Transform.Position);
        _shader.SetVector3("lightPos", LightPos);
        
        Matrix4 view = _camera.GetViewMatrix();
        Matrix4 projection = _camera.GetPerspectiveMatrix();
        
        foreach (GameObject gameObject in _scene)
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
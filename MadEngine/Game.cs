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
    private double _deltaTime;
    private Camera _camera = new();

    public float Height;
    public float Width;
    
    public Game(int width, int height, string title) : base(new GameWindowSettings()
    {
        UpdateFrequency = 60
    },
        new NativeWindowSettings()
    {
        ClientSize = (width, height), Title = title

    })
    {
        Width = width;
        Height = height;
        _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

        float[] vertices = {
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,
            0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
            0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
            0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
            0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

            -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

            0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
            0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
            0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
            0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
        };
        
        uint[] indices = {
            0,  1,  2,  3,  4,  5,
            6,  7,  8,  9, 10, 11,
            12, 13, 14, 15, 16, 17,
            18, 19, 20, 21, 22, 23,
            24, 25, 26, 27, 28, 29,
            30, 31, 32, 33, 34, 35
        };
        MeshRenderer meshRenderer1 = new MeshRenderer(new Mesh(vertices, indices), new Material(_shader, new Texture("Textures/container.jpg"), new Vector4(1f, 1f, 1f, 1f)));
        
        Transform transform = new Transform();
        transform.Position = new Vector3(0f, 0f, 0f);
        
        _scene = [new GameObject(meshRenderer1, transform)];
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

    protected override void OnUnload()
    {
        base.OnUnload();

        _shader.Dispose();
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        _deltaTime = UpdateTime;
        base.OnRenderFrame(args);

        _scene[0].Transform.Rotation.Y += 60 * (float)_deltaTime;
        _scene[0].Transform.Rotation.X += 60 * (float)_deltaTime;
        
        GL.Clear(ClearBufferMask.DepthBufferBit);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        Matrix4 view = Matrix4.CreateTranslation(Position.Position.X, Position.Position.Y, Position.Position.Z);
        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), Width / Height, 0.1f, 100f);
        _shader.SetMatrix4("view", view);
        _shader.SetMatrix4("projection", projection);
        
        foreach (GameObject gameObject in _scene)
        {
            gameObject.MeshRenderer.Draw();
        }
        
        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs args)
    {
        base.OnUpdateFrame(args);

        if (KeyboardState.IsKeyDown(Keys.Escape))
        {
            Close();
        }

        if (!IsFocused)
        {
            return;
        }

        KeyboardState input = KeyboardState;
        float speed = _camera.speed * (float)args.Time;
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);
        Width = e.Width;
        Height = e.Height;
        GL.Viewport(0, 0, e.Width, e.Height);
    }
}
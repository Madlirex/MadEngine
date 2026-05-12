using System.Numerics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;


namespace MadEngine;

public class Game : GameWindow
{
    private GameObject[] _scene;
    private Shader _shader;
    private double _deltaTime;

    public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings()
    {
        ClientSize = (width, height), Title = title

    })
    {
        _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

        float[] vertices =
        [
            0.5f, 0.5f, 0.0f, // top right
            0.5f, -0.5f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, // bottom left
            -0.5f, 0.5f, 0.0f
        ];
        
        uint[] indices =
        [
            0, 1, 3,   // first triangle
            1, 2, 3    // second triangle
        ];
        MeshRenderer meshRenderer1 = new MeshRenderer(new Mesh(vertices, indices), new Material(_shader, new Vector4(1f, 0f, 0f, 1f)));

        float[] vertices2 =
        [
            0.8f, 0.8f, 0.5f,
            0.8f, -0.8f, 0f,
            -0.8f, -0.8f, 0f,
            -0.8f, 0.8f, 0f
        ];

        uint[] indices2 =
        [
            0, 1, 3,
            1, 2, 3
        ];
        MeshRenderer meshRenderer2 = new MeshRenderer(new Mesh(vertices2, indices2), new Material(_shader, new Vector4(0f, 0f, 1f, 1f)));
        
        _scene = [new GameObject(meshRenderer1), new GameObject(meshRenderer2)];
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
        
        GL.Clear(ClearBufferMask.ColorBufferBit);

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
    }

    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        base.OnFramebufferResize(e);
        
        GL.Viewport(0, 0, e.Width, e.Height);
    }
}
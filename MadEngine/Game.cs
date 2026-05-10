using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;


namespace MadEngine;

public class Game : GameWindow
{
    private Mesh[] _triangles = [new([-0.5f, -0.5f, 0.0f, 0.5f, -0.5f, 0.0f, 0.0f, 0.5f, 0.0f]),
                                new ([-0.75f, -0.5f, 0.0f, -0.65f, -0.5f, 0.0f, 0.0f, 0.5f, 0.0f])];
    private Shader _shader;

    public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings()
    {
        ClientSize = (width, height), Title = title

    })
    {
        _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
    }

    protected override void OnLoad()
    {
        base.OnLoad();

        foreach (Mesh triangle in _triangles)
        {
            triangle.Initialize();
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
        base.OnRenderFrame(args);
        
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        _shader.Use();

        foreach (Mesh triangle in _triangles)
        {
            triangle.Draw();
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
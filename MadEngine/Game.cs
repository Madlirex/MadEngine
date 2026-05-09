using System.ComponentModel;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;


namespace MadEngine;

public class Game : GameWindow
{
    private int _vertexBufferObject;
    private Shape _triangle = new Shape([-0.5f, -0.5f, 0.0f, 0.5f, -0.5f, 0.0f, 0.0f, 0.5f, 0.0f]);
    private Shader _shader;
    
    public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings()
    {
        ClientSize = (width, height), Title = title 
        
    }) { }

    protected override void OnLoad()
    {
        base.OnLoad();
        
        _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
        
        _vertexBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, _triangle.Vertices.Length * sizeof(float), _triangle.Vertices, BufferUsageHint.StaticDraw);
        
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1f);

        
    }

    protected override void OnUnload()
    {
        base.OnUnload();

        _shader.Dispose();
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        GL.DeleteBuffer(_vertexBufferObject);
    }

    protected override void OnRenderFrame(FrameEventArgs args)
    {
        base.OnRenderFrame(args);
        
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
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
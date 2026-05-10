using OpenTK.Graphics.OpenGL4;

namespace MadEngine;

public class Mesh : IDisposable
{
    public float[] Vertices { get; }

    private int _vertexBufferObject;
    private int _vertexArrayObject;

    private bool _initialized;
    private bool _disposedValue;

    public Mesh(float[] vertices)
    {
        Vertices = vertices;
    }

    public void Initialize()
    {
        if (_initialized) return;
        
        _vertexArrayObject = GL.GenVertexArray();
        _vertexBufferObject = GL.GenBuffer();
        
        GL.BindVertexArray(_vertexArrayObject);
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, 
            Vertices.Length * sizeof(float), 
            Vertices, 
            BufferUsageHint.StaticDraw);
        
        GL.VertexAttribPointer(0, 
            3, 
            VertexAttribPointerType.Float, 
            false, 
            sizeof(float) * 3, 0);
        GL.EnableVertexAttribArray(0);
        
        GL.BindVertexArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        
        _initialized = true;
    }

    public void Draw()
    {
        GL.BindVertexArray(_vertexArrayObject);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
    }

    public void Dispose()
    {
        if (_disposedValue) return;
        
        GL.DeleteBuffer(_vertexBufferObject);
        GL.DeleteVertexArray(_vertexArrayObject);
        
        _disposedValue = true;
    }
}
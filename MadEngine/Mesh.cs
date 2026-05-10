using OpenTK.Graphics.OpenGL4;

namespace MadEngine;

public class Mesh : IDisposable
{
    public float[] Vertices { get; }
    public uint[]  Indices { get; }

    private int _vertexBufferObject;
    private int _vertexArrayObject;
    private int _elementBufferObject;

    private bool _initialized;
    private bool _disposedValue;

    public Mesh(float[] vertices, uint[] indices)
    {
        Vertices = vertices;
        Indices = indices;
    }

    public void Initialize()
    {
        if (_initialized) return;
        
        _vertexArrayObject = GL.GenVertexArray();
        _vertexBufferObject = GL.GenBuffer();
        _elementBufferObject = GL.GenBuffer();
        
        GL.BindVertexArray(_vertexArrayObject);
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, 
            Vertices.Length * sizeof(float), 
            Vertices, 
            BufferUsageHint.StaticDraw);
        
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer,
            Indices.Length * sizeof(uint),
            Indices,
            BufferUsageHint.StaticDraw);
        
        GL.VertexAttribPointer(0,  // Can use GL.GetAttribLocation(Handle, "aPosition") instead of hard-coded values - in future
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
        GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
    }

    public void Dispose()
    {
        if (_disposedValue) return;
        
        GL.DeleteBuffer(_vertexBufferObject);
        GL.DeleteVertexArray(_vertexArrayObject);
        
        _disposedValue = true;
    }
}
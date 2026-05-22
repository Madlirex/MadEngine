using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace MadEngine;

public class Mesh : IDisposable
{
    public Vertex[] Vertices { get; }
    public uint[] Indices { get; }
    
    private int _vertexBufferObject;
    private int _vertexArrayObject;
    private int _elementBufferObject;

    private bool _initialized;
    private bool _disposedValue;

    public Mesh(float[] vertices, uint[] indices)
    {
        const int stride = 8;

        Vertices = new Vertex[vertices.Length / stride];
        
        for (int i = 0, v = 0; i < vertices.Length; i += stride, v++)
        {
            Vertices[v] = new Vertex
            {
                Position = new Vector3(
                    vertices[i],
                    vertices[i + 1],
                    vertices[i + 2]
                ),

                TexCoord = new Vector2(
                    vertices[i + 3],
                    vertices[i + 4]
                ),

                Normal = new Vector3(
                    vertices[i+5],
                    vertices[i+6],
                    vertices[i+7]
                )
            };
        }
        
        Indices = indices;
        CalculateNormals(this);
    }

    public void Initialize()
    {
        if (_initialized) return;
        
        _vertexArrayObject = GL.GenVertexArray();
        _vertexBufferObject = GL.GenBuffer();
        _elementBufferObject = GL.GenBuffer();
        
        GL.BindVertexArray(_vertexArrayObject);
        
        int vertexSize = Marshal.SizeOf<Vertex>();
        
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, 
            Vertices.Length * vertexSize, 
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
            vertexSize, Marshal.OffsetOf<Vertex>(nameof(Vertex.Position)));
        GL.EnableVertexAttribArray(0);
        
        GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, vertexSize, Marshal.OffsetOf<Vertex>(nameof(Vertex.TexCoord)));
        GL.EnableVertexAttribArray(1);
        
        GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, vertexSize, Marshal.OffsetOf<Vertex>(nameof(Vertex.Normal)));
        GL.EnableVertexAttribArray(2);
        
        GL.BindVertexArray(0);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        
        _initialized = true;
    }

    public void Draw()
    {
        GL.BindVertexArray(_vertexArrayObject);
        GL.DrawArrays(PrimitiveType.Triangles, 0, Vertices.Length);
    }

    public void Dispose()
    {
        if (_disposedValue) return;
        
        GL.DeleteBuffer(_vertexBufferObject);
        GL.DeleteVertexArray(_vertexArrayObject);
        GL.DeleteBuffer(_elementBufferObject);
        
        _disposedValue = true;
    }
    
    public static void CalculateNormals(Mesh mesh)
    {
        var vertices = mesh.Vertices;
        var indices = mesh.Indices;
        
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].Normal = Vector3.Zero;
        }
        
        for (int i = 0; i < indices.Length; i += 3)
        {
            int i0 = (int)indices[i];
            int i1 = (int)indices[i + 1];
            int i2 = (int)indices[i + 2];

            ref var v0 = ref vertices[i0];
            ref var v1 = ref vertices[i1];
            ref var v2 = ref vertices[i2];

            Vector3 edge1 = v1.Position - v0.Position;
            Vector3 edge2 = v2.Position - v0.Position;

            Vector3 faceNormal = Vector3.Cross(edge1, edge2);

            v0.Normal += faceNormal;
            v1.Normal += faceNormal;
            v2.Normal += faceNormal;
        }
        
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].Normal = Vector3.Normalize(vertices[i].Normal);
            Console.WriteLine($"{vertices[i].Position}, {vertices[i].Normal}");
        }
    }
}
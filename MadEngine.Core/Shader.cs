using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace MadEngine.Core;

public class Shader : IDisposable
{
    private int _handle;
    private bool _disposedValue;

    public Shader(string vertexPath, string fragmentPath)
    {
        string vertexShaderSource = File.ReadAllText(vertexPath);
        string fragmentShaderSource = File.ReadAllText(fragmentPath);
        
        int vertexShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertexShader, vertexShaderSource);
        
        int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragmentShader, fragmentShaderSource);
        
        GL.CompileShader(vertexShader);

        GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetShaderInfoLog(vertexShader);
            Console.WriteLine(infoLog);
        }

        GL.CompileShader(fragmentShader);

        GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out success);
        if (success == 0)
        {
            string infoLog = GL.GetShaderInfoLog(fragmentShader);
            Console.WriteLine(infoLog);
        }
        
        _handle = GL.CreateProgram();

        GL.AttachShader(_handle, vertexShader);
        GL.AttachShader(_handle, fragmentShader);

        GL.LinkProgram(_handle);

        GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out success);
        if (success == 0)
        {
            string infoLog = GL.GetProgramInfoLog(_handle);
            Console.WriteLine(infoLog);
        }
        
        GL.DetachShader(_handle, vertexShader);
        GL.DetachShader(_handle, fragmentShader);
        GL.DeleteShader(fragmentShader);
        GL.DeleteShader(vertexShader);
    }

    public void Use()
    {
        GL.UseProgram(_handle);
    }

    public void SetVector4(string name, float x, float y, float z, float w)
    {
        int location = GL.GetUniformLocation(_handle, name);
        GL.Uniform4(location, x, y, z, w);
    }
    
    public void SetVector4(string name, Vector4 v)
    {
        SetVector4(name, v.X, v.Y, v.Z, v.W);
    }

    public void SetMatrix4(string name, Matrix4 matrix)
    {
        int location = GL.GetUniformLocation(_handle, name);
        GL.UniformMatrix4(location, true, ref matrix);
    }

    public void SetVector3(string name, Vector3 v)
    {
        GL.Uniform3(GL.GetUniformLocation(_handle, name), v);
    }

    public void SetInt(string name, int value)
    {
        GL.Uniform1(GL.GetUniformLocation(_handle, name), value);
    }
    
    public void SetFloat(string name, float value)
    {
        GL.Uniform1(GL.GetUniformLocation(_handle, name), value);
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        GL.DeleteProgram(_handle);

        _disposedValue = true;
    }

    ~Shader()
    {
        if (!_disposedValue)
        {
            Console.WriteLine("GPU Resource leak! Did you forget to call Dispose()?");
        }
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
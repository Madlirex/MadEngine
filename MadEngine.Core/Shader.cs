using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace MadEngine.Core;

public class Shader : Asset
{
    [DoNotSave] private bool _initialized;
    [DoNotSave] private int _handle;

    public string VertexPath = "";
    public string FragmentPath = "";

    public override string Name { get; set; } = "NewShader";
    public override string Extension => ".shader";

    public Shader(string vertexPath, string fragmentPath)
    {
        VertexPath = vertexPath;
        FragmentPath = fragmentPath;
        Initialize();
    }

    public Shader()
    {
        
    }

    public void Initialize()
    {
        if(_initialized) return;
        string vertexShaderSource = File.ReadAllText(VertexPath);
        string fragmentShaderSource = File.ReadAllText(FragmentPath);
        
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
        _initialized = true;
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
    
    protected override void OnDispose(bool disposing)
    {
        GL.DeleteProgram(_handle);
        
        base.OnDispose(disposing);
    }

    ~Shader()
    {
        if (!Disposed)
        {
            Console.WriteLine("GPU Resource leak! Did you forget to call Dispose()?");
        }
    }
}
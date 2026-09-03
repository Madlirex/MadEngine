using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using StbImageSharp;

namespace MadEngine.Core;

public class FlatBackground : IDisposable
{
    private int _vao;
    private int _shaderProgram;
    private bool _isInitialized;
    
    private const string VertexShaderSource = @"
        #version 330 core
        out vec2 ScreenPos;
        void main()
        {
            float x = -1.0 + float((gl_VertexID & 1) << 2);
            float y = -1.0 + float((gl_VertexID & 2) << 1);
            
            gl_Position = vec4(x, y, 1.0, 1.0);
            ScreenPos = vec2(x, y);
        }";
    
    private const string FragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;
        in vec2 ScreenPos;

        uniform mat4 invProjection;
        uniform mat4 invView;

        const vec3 SkyColor   = vec3(0.3f, 0.5f, 0.7f); 
        const vec3 HorizonColor = vec3(0.65f, 0.75f, 0.85f);
        const vec3 GroundColor  = vec3(0.24f, 0.21f, 0.20f);

        void main()
        {
            vec4 unprojected = invProjection * vec4(ScreenPos, 1.0, 1.0);
            vec4 worldDir = invView * vec4(unprojected.xyz / unprojected.w, 0.0);
            vec3 rayDir = normalize(worldDir.xyz);

            float height = rayDir.y;

            vec3 finalColor;
            if (height >= 0.0)
            {
                float skyBlend = pow(height, 0.5); 
                finalColor = mix(HorizonColor, SkyColor, skyBlend);
            }
            else
            {
                float groundBlend = clamp(-height * 10.0, 0.0, 1.0);
                finalColor = mix(HorizonColor, GroundColor, groundBlend);
            }

            FragColor = vec4(finalColor, 1.0);
        }";

    public void Initialize()
    {
        if (_isInitialized) return;
        
        _vao = GL.GenVertexArray();
        
        int vertexShader = CompileShader(ShaderType.VertexShader, VertexShaderSource);
        int fragmentShader = CompileShader(ShaderType.FragmentShader, FragmentShaderSource);

        _shaderProgram = GL.CreateProgram();
        GL.AttachShader(_shaderProgram, vertexShader);
        GL.AttachShader(_shaderProgram, fragmentShader);
        GL.LinkProgram(_shaderProgram);
        
        GL.GetProgram(_shaderProgram, GetProgramParameterName.LinkStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetProgramInfoLog(_shaderProgram);
            throw new Exception($"Error linking procedural background shader: {infoLog}");
        }
        
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        _isInitialized = true;
    }

    public void Render(Matrix4 viewMatrix, Matrix4 projectionMatrix)
    {
        if (!_isInitialized) return;

        GL.DepthFunc(DepthFunction.Lequal);
        GL.UseProgram(_shaderProgram);
        
        Matrix4 rotationOnlyView = new Matrix4(new Matrix3(viewMatrix));
        
        Matrix4 invProjection = projectionMatrix.Inverted();
        Matrix4 invView = rotationOnlyView.Inverted();
        
        GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "invProjection"), false, ref invProjection);
        GL.UniformMatrix4(GL.GetUniformLocation(_shaderProgram, "invView"), false, ref invView);
        
        GL.BindVertexArray(_vao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

        GL.DepthFunc(DepthFunction.Less);
    }

    private int CompileShader(ShaderType type, string source)
    {
        int shader = GL.CreateShader(type);
        GL.ShaderSource(shader, source);
        GL.CompileShader(shader);

        GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            throw new Exception($"Error compiling background {type}: {GL.GetShaderInfoLog(shader)}");
        }
        return shader;
    }

    public void Dispose()
    {
        if (_isInitialized)
        {
            GL.DeleteVertexArray(_vao);
            GL.DeleteProgram(_shaderProgram);
        }
    }
}

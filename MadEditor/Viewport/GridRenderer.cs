using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace MadEditor;

public class SceneGridRenderer : IDisposable
{
    private int _vao;
    private int _vbo;
    private int _shaderProgram;
    private bool _disposed;
    
    private const string VertexShaderSource = @"
        #version 330 core
        layout(location = 0) in vec3 inPosition;

        uniform mat4 view;
        uniform mat4 projection;

        out vec3 nearPoint;
        out vec3 farPoint;

        vec3 UnprojectPoint(float x, float y, float z, mat4 view, mat4 projection) {
            mat4 viewInv = inverse(view);
            mat4 projInv = inverse(projection);
            vec4 unprojectedPoint = viewInv * projInv * vec4(x, y, z, 1.0);
            return unprojectedPoint.xyz / unprojectedPoint.w;
        }

        void main() {
            nearPoint = UnprojectPoint(inPosition.x, inPosition.y, -1.0, view, projection);
            farPoint = UnprojectPoint(inPosition.x, inPosition.y, 1.0, view, projection);
            
            gl_Position = vec4(inPosition, 1.0);
        }
    ";

    private const string FragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;

        in vec3 nearPoint;
        in vec3 farPoint;

        out float gl_FragDepth;

        uniform mat4 view;
        uniform mat4 projection;
        uniform float cameraFarPlane;
        uniform vec3 cameraWorldPos;

        vec4 gridColorBase = vec4(0.55, 0.55, 0.55, 1.0); 
        vec4 xAxisColor = vec4(0.95, 0.25, 0.25, 1.0);
        vec4 zAxisColor = vec4(0.25, 0.55, 0.95, 1.0);

        float log10(float x) {
            return log2(x) * 0.30102999566;
        }

        float ComputeGridFactor(vec2 st, vec2 derivative) {
            vec2 grid = abs(fract(st - 0.5) - 0.5) / derivative;
            float line = min(grid.x, grid.y);
            return 1.0 - min(line, 1.0);
        }

        void main() {
            float t = -nearPoint.y / (farPoint.y - nearPoint.y);
            if (t < 0.0) discard;

            vec3 fragPos3D = nearPoint + t * (farPoint - nearPoint);

            vec4 clipSpacePos = projection * view * vec4(fragPos3D, 1.0);
            gl_FragDepth = (clipSpacePos.z / clipSpacePos.w + 1.0) / 2.0;

            float near = 0.1; 
            float linearDepth = (2.0 * near * cameraFarPlane) / (cameraFarPlane + near - (gl_FragDepth * 2.0 - 1.0) * (cameraFarPlane - near));
            
            float fadeStart = cameraFarPlane * 0.4;
            float horizonFade = 1.0 - smoothstep(fadeStart, cameraFarPlane, linearDepth);

            float distToCam = distance(cameraWorldPos, fragPos3D);
            
            float logDist = log10(distToCam * 0.15); 
            float cellLevel = floor(logDist);
            float cellLevelFrac = fract(logDist);

            float scaleMinimum = pow(10.0, cellLevel);
            float scaleMedium  = scaleMinimum * 10.0;
            float scaleMaximum = scaleMedium * 10.0;

            vec2 derivativeMin = fwidth(fragPos3D.xz / scaleMinimum);
            vec2 derivativeMed = fwidth(fragPos3D.xz / scaleMedium);
            vec2 derivativeMax = fwidth(fragPos3D.xz / scaleMaximum);

            float gridMin = ComputeGridFactor(fragPos3D.xz / scaleMinimum, derivativeMin);
            float gridMed = ComputeGridFactor(fragPos3D.xz / scaleMedium, derivativeMed);
            float gridMax = ComputeGridFactor(fragPos3D.xz / scaleMaximum, derivativeMax);

            float finalGridIntensity = 0.0;
            if (cellLevelFrac < 0.5) {
                finalGridIntensity = mix(gridMin, gridMed, smoothstep(0.0, 0.5, cellLevelFrac));
            } else {
                finalGridIntensity = mix(gridMed, gridMax, smoothstep(0.5, 1.0, cellLevelFrac));
            }

            vec4 finalColor = gridColorBase * finalGridIntensity;

            vec2 axisWidth = fwidth(fragPos3D.xz);
            float axisThicknessMultiplier = 1.5;
            
            float x_line = smoothstep(axisWidth.y * axisThicknessMultiplier, 0.0, abs(fragPos3D.z));
            float z_line = smoothstep(axisWidth.x * axisThicknessMultiplier, 0.0, abs(fragPos3D.x));

            if (x_line > 0.0) finalColor = mix(finalColor, xAxisColor, x_line);
            if (z_line > 0.0) finalColor = mix(finalColor, zAxisColor, z_line);

            finalColor.a *= horizonFade;
            if (finalColor.a < 0.01) discard;

            FragColor = finalColor;
        }
    ";

    public SceneGridRenderer()
    {
        InitializeRenderingData();
    }

    private void InitializeRenderingData()
    {
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
            throw new Exception($"Error linking grid shader program: {infoLog}");
        }

        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);

        float[] quadVertices =
        [
            -1.0f,  1.0f, 0.0f,
            -1.0f, -1.0f, 0.0f,
             1.0f, -1.0f, 0.0f,

            -1.0f,  1.0f, 0.0f,
             1.0f, -1.0f, 0.0f,
             1.0f,  1.0f, 0.0f
        ];

        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();

        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, quadVertices.Length * sizeof(float), quadVertices, BufferUsageHint.StaticDraw);

        GL.EnableVertexAttribArray(0);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

        GL.BindVertexArray(0);
    }

    public void Render(Matrix4 viewMatrix, Matrix4 projectionMatrix, Vector3 cameraWorldPosition, float farPlaneDistance)
    {
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        
        GL.Enable(EnableCap.DepthTest); 
        GL.DepthMask(true);

        GL.UseProgram(_shaderProgram);
        
        int viewLoc = GL.GetUniformLocation(_shaderProgram, "view");
        int projLoc = GL.GetUniformLocation(_shaderProgram, "projection");
        int farPlaneLoc = GL.GetUniformLocation(_shaderProgram, "cameraFarPlane");
        int camPosLoc = GL.GetUniformLocation(_shaderProgram, "cameraWorldPos");

        GL.UniformMatrix4(viewLoc, false, ref viewMatrix);
        GL.UniformMatrix4(projLoc, false, ref projectionMatrix);
        GL.Uniform1(farPlaneLoc, farPlaneDistance);
        GL.Uniform3(camPosLoc, cameraWorldPosition);

        GL.BindVertexArray(_vao);
        GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        
        GL.BindVertexArray(0);
        GL.Disable(EnableCap.Blend);
    }

    private int CompileShader(ShaderType type, string source)
    {
        int shader = GL.CreateShader(type);
        GL.ShaderSource(shader, source);
        GL.CompileShader(shader);

        GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            string infoLog = GL.GetShaderInfoLog(shader);
            throw new Exception($"Error compiling {type}: {infoLog}");
        }
        return shader;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            GL.DeleteVertexArray(_vao);
            GL.DeleteBuffer(_vbo);
            GL.DeleteProgram(_shaderProgram);
            _disposed = true;
            GC.SuppressFinalize(this);
        }
    }

    ~SceneGridRenderer() => Dispose();
}

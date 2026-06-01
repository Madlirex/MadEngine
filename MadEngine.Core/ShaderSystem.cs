namespace MadEngine.Core;

public static class ShaderSystem
{
    public static Shader UnlitShader { get; private set; }
    public static Shader LitShader { get; private set; }

    private static List<Shader> _shaders;
    public static IReadOnlyList<Shader> Shaders => _shaders;

    public static void RegisterShader(Shader shader)
    {
        _shaders.Add(shader);
    }

    public static void Dispose()
    {
        foreach (Shader shader in _shaders)
        {
            shader.Dispose();
        }
        _shaders.Clear();
    }

    public static void InitializeUnlitShader(string vertPath, string fragPath)
    {
        UnlitShader = new Shader(vertPath, fragPath);
        RegisterShader(UnlitShader);
    }

    public static void InitializeLitShader(string vertPath, string fragPath)
    {
        LitShader = new Shader(vertPath, fragPath);
        RegisterShader(LitShader);
    }
}
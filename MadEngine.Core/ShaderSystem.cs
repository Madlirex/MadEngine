namespace MadEngine.Core;

public static class ShaderSystem
{
    private static List<Shader> _shaders = [];
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
}
namespace MadEngine.Core.SceneManagement;

public static class SceneManager
{
    public static Scene ActiveScene => _activeScene ??= new Scene();
    private static Scene? _activeScene;
    public static List<Scene> Scenes = [];

    public static void LoadScene(Scene scene)
    {
        _activeScene = scene;
    }

    public static void LoadScene(int index)
    {
        _activeScene = Scenes[index];
    }
}
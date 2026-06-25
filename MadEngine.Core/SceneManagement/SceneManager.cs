namespace MadEngine.Core.SceneManagement;

public static class SceneManager
{
    public static Scene ActiveScene;
    public static List<Scene> Scenes;

    public static void LoadScene(Scene scene)
    {
        ActiveScene = scene;
    }

    public static void LoadScene(int index)
    {
        ActiveScene = Scenes[index];
    }
}
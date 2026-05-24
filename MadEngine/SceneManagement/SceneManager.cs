namespace MadEngine.SceneManagement;

public static class SceneManager
{
    public static Scene ActiveScene;
    public static Scene[] Scenes;

    public static void LoadScene(Scene scene)
    {
        ActiveScene = scene;
    }

    public static void LoadScene(int index)
    {
        ActiveScene = Scenes[index];
    }
}
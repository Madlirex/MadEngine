namespace MadEngine.SceneManagement;

public class SceneManager
{
    public Scene ActiveScene;
    public Scene[] Scenes;

    public void LoadScene(Scene scene)
    {
        ActiveScene = scene;
    }

    public void LoadScene(int index)
    {
        ActiveScene = Scenes[index];
    }
}
namespace MadEngine.SceneManagement;

public class Scene
{
    public readonly  List<GameObject> GameObjects = [];

    public void Add(GameObject gameObject)
    {
        GameObjects.Add(gameObject);
    }
}
namespace MadEngine.SceneManagement;

public class Scene
{
    public readonly List<GameObject> GameObjects = [];
    public List<Light> Lights = [];
    public List<MeshRenderer> MeshRenderers = [];

    public void Add(GameObject gameObject)
    {
        GameObjects.Add(gameObject);
    }

    public void Register(GameObject gameObject)
    {
        foreach (Component component in gameObject.Components)
        {
            if (component is Light light)
            {
                
            }
        }
    }
}
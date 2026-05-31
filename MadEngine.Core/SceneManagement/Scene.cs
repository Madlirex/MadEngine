namespace MadEngine.Core.SceneManagement;

public class Scene
{
    public readonly List<GameObject> GameObjects = [];
    public List<Light> Lights = [];
    public List<MeshRenderer> MeshRenderers = [];

    public void Add(GameObject gameObject)
    {
        GameObjects.Add(gameObject);
        Register(gameObject);
    }

    public void Register(GameObject gameObject)
    {
        foreach (Component component in gameObject.Components)
        {
            RegisterComponent(component);
        }
    }

    public void RegisterComponent(Component component)
    {
        if (component is Light light)
        {
            Lights.Add(light);
        }

        if (component is MeshRenderer meshRenderer)
        {
            MeshRenderers.Add(meshRenderer);
            meshRenderer.Mesh.Initialize();
        }
    }
}
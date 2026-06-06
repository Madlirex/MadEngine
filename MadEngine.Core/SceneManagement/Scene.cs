namespace MadEngine.Core.SceneManagement;

public class Scene
{
    public string Name = "NewScene";
    
    private readonly List<GameObject> _gameObjects = [];
    public IReadOnlyList<GameObject> GameObjects => _gameObjects;

    private readonly List<Light> _lights = [];
    public IReadOnlyList<Light> Lights => _lights;

    private readonly List<MeshRenderer> _meshRenderers = [];
    public IReadOnlyList<MeshRenderer> MeshRenderers => _meshRenderers;

    public void Add(GameObject gameObject)
    {
        _gameObjects.Add(gameObject);
        Register(gameObject);
    }

    public void Register(GameObject gameObject)
    {
        foreach (Component component in gameObject.Components)
        {
            RegisterComponent(component);
        }

        gameObject.ComponentAdded += RegisterComponent;
        gameObject.ComponentRemoved += UnregisterComponent;
    }

    public void RegisterComponent(Component component)
    {
        if (component is Light light)
        {
            _lights.Add(light);
        }

        if (component is MeshRenderer meshRenderer)
        {
            _meshRenderers.Add(meshRenderer);
            meshRenderer.Mesh.Initialize();
        }
    }

    public void UnregisterComponent(Component component)
    {
        if (component is Light light)
        {
            _lights.Remove(light);
        }

        if (component is MeshRenderer meshRenderer)
        {
            _meshRenderers.Remove(meshRenderer);
        }
    }
}
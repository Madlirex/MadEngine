namespace MadEngine.Core.SceneManagement;

public class Scene : Asset
{
    private readonly List<GameObject> _gameObjects = [];
    public IReadOnlyList<GameObject> GameObjects => _gameObjects;

    private readonly List<Light> _lights = [];
    public IReadOnlyList<Light> Lights => _lights;

    private readonly List<Renderer> _renderers = [];
    public IReadOnlyList<Renderer> Renderers => _renderers;
    
    public override string Name { get; set; } = "NewScene";
    public override string Extension => ".madscene";

    public Scene()
    {
        SceneManager.Scenes.Add(this);
        Console.WriteLine(SceneManager.Scenes.Count);
    }

    ~Scene()
    {
        SceneManager.Scenes.Remove(this);
    }
    
    public void Add(GameObject gameObject)
    {
        _gameObjects.Add(gameObject);
        Register(gameObject);
    }

    public void Destroy(GameObject gameObject)
    {
        gameObject.Destroy();
        _gameObjects.Remove(gameObject);
        Unregister(gameObject);
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

    public void Unregister(GameObject gameObject)
    {
        foreach (Component component in gameObject.Components)
        {
            UnregisterComponent(component);
        }

        gameObject.ComponentAdded -= RegisterComponent;
        gameObject.ComponentRemoved -= UnregisterComponent;
    }

    public void RegisterComponent(Component component)
    {
        if (component is Light light)
        {
            _lights.Add(light);
        }

        if (component is Renderer renderer)
        {
            _renderers.Add(renderer);
        }
    }

    public void UnregisterComponent(Component component)
    {
        if (component is Light light)
        {
            _lights.Remove(light);
        }

        if (component is Renderer renderer)
        {
            _renderers.Remove(renderer);
        }
    }
}
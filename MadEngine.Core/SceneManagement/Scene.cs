namespace MadEngine.Core.SceneManagement;

public class Scene : Asset
{
    public Camera MainCamera
    {
        get
        {
            Console.WriteLine(AssetRegistry.ObjectMap.Count);
            if (_mainCamera != null) return _mainCamera;
            
            Camera? cam = GameObject.FindFirstComponent<Camera>();
            if (cam is { } camera)
            {
                cam.IsMainCameraInstance = true;
                _mainCamera = camera;
                return cam;
            }

            GameObject obj = new GameObject();
            cam = (Camera?)obj.AddComponentUnsafe(new Camera());
            if (cam is not { } cameraComp)
            {
                _mainCamera = new Camera();
                return _mainCamera;
            }
            AddObjectUnsafe(obj);
            _mainCamera = cameraComp;
            
            return cameraComp;

        }

        internal set => _mainCamera = value;
    }

    public void ClearMainCamera() => _mainCamera = null;
    public Camera? GetMainCamera() => _mainCamera;

    private Camera? _mainCamera;
    
    private readonly List<GameObject> _gameObjects = [];
    public IReadOnlyList<GameObject> GameObjects => _gameObjects;

    [DoNotSave] private readonly List<Light> _lights = [];
    public IReadOnlyList<Light> Lights => _lights;

    [DoNotSave] private readonly List<Renderer> _renderers = [];
    public IReadOnlyList<Renderer> Renderers => _renderers;
    
    protected override string NameInternal { get; set; } = "NewScene";
    protected override string ExtensionInternal { get; set; } = ".madscene";

    public Scene()
    {
        SceneManager.Scenes.Add(this);
    }

    protected override void OnDispose(bool disposing)
    {
        if (disposing)
        {
            foreach(GameObject obj in GameObjects)
                obj.Destroy();
            
            SceneManager.Scenes.Remove(this);
        }
        base.OnDispose(disposing);
    }
    
    public void Add(GameObject gameObject)
    {
        EngineCommandsManager.Enqueue(new AddGameObjectCommand(gameObject));
    }

    public void Destroy(GameObject gameObject)
    {
        EngineCommandsManager.Enqueue(new DestroyGameObjectCommand(gameObject));
    }

    public void AddObjectUnsafe(GameObject gameObject) => AddObjectSafe(gameObject);
    
    internal void AddObjectSafe(GameObject gameObject)
    {
        if (_gameObjects.Contains(gameObject)) return;
    
        _gameObjects.Add(gameObject);
        Register(gameObject);
        
        var children = gameObject.Transform.Children;
        for (int i = 0; i < children.Count; i++)
        {
            AddObjectSafe(children[i].GameObject);
        }
    }

    public void DestroyObjectUnsafe(GameObject gameObject) => DestroyObjectSafe(gameObject);
    
    internal void DestroyObjectSafe(GameObject gameObject)
    {
        _gameObjects.Remove(gameObject);
        Unregister(gameObject);
        
        var children = gameObject.Transform.Children;
        for (int i = children.Count - 1; i >= 0; i--)
        {
            DestroyObjectSafe(children[i].GameObject);
        }

        gameObject.Transform.Parent = null;

        gameObject.Destroy();
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
        switch (component)
        {
            case Light light:
                _lights.Add(light);
                break;
            case Renderer renderer:
                _renderers.Add(renderer);
                break;
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
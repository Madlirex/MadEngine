using MadEngine.Core.SceneManagement;

namespace MadEngine.Core;

public class GameObject : MadObject
{
    public Transform Transform;

    public IReadOnlyList<Component> Components => _components;
    private List<Component> _components = [];

    public event Action<Component>? ComponentAdded;
    public event Action<Component>? ComponentRemoved;

    public GameObject()
    {
        Name = "NewGameObject";
        Transform = new();
        _components.Add(Transform);
        Transform.AssignGameObject(this);
    }

    public void Awake()
    {
        foreach (Component component in Components)
            component.Awake();
    }

    public void Start()
    {
        foreach (Component component in Components)
            component.Start();
    }

    public void Update(float deltaTime)
    {
        foreach (Component component in Components)
            component.Update(deltaTime);
    }

    protected override void OnDispose(bool disposing)
    {
        if (disposing)
        {
            foreach (Component component in Components)
            {
                component.OnDestroy();
            }

            var componentsArray = Components.ToArray();
            foreach (Component component in componentsArray)
            {
                component.Destroy();
            }

            _components.Clear();
        }

        base.OnDispose(disposing);
    }

    public void EditorStart()
    {
        foreach (Component component in Components)
        {
            component.EditorStart();
        }
    }

    public void EditorUpdate(float deltaTime)
    {
        foreach (Component component in Components)
            component.EditorUpdate(deltaTime);
    }

    public T? AddComponent<T>() where T : Component
    {
        if (!ComponentRules.CanBeAdded(typeof(T)))
            return null;

        return (T?)AddComponent(typeof(T));
    }

    public Component? AddComponent(Type type)
    {
        if (!ComponentRules.CanBeAdded(type))
            return null;

        Component component = (Component)Activator.CreateInstance(type)!;
        return AddComponent(component);
    }

    public Component? AddComponent(Component component)
    {
        if (!ComponentRules.CanBeAdded(component.GetType()))
            return null;

        EngineCommandsManager.Enqueue(new AddComponentCommand(this, component));
        return component;
    }

    public bool RemoveComponent(Type type)
    {
        if (!ComponentRules.CanBeRemoved(type))
            return false;

        Component? component = GetComponent(type);
        if (component == null)
            return false;
        
        if (!_components.Contains(component))
            return false;

        EngineCommandsManager.Enqueue(new RemoveComponentCommand(this, component));

        return true;
    }

    public bool RemoveComponent(Component component)
    {
        if (!ComponentRules.CanBeRemoved(component.GetType()))
            return false;

        if (!_components.Contains(component))
            return false;
        
        EngineCommandsManager.Enqueue(new RemoveComponentCommand(this, component));

        return true;
    }
    
    public bool RemoveComponent<T>() where T : Component
    {
        return RemoveComponent(typeof(T));
    }

    internal Component? AddComponentSafe(Component component)
    {
        if (!ComponentRules.CanBeAdded(component.GetType()))
            return null;
        
        _components.Add(component);
        component.AssignGameObject(this);
        
        ComponentAdded?.Invoke(component);
        return component;
    }

    internal bool RemoveComponentSafe(Component? component)
    {
        if (component == null)
            return false;
        
        if (!ComponentRules.CanBeRemoved(component.GetType()))
            return false;

        if(!_components.Remove(component))
            return false;
        
        ComponentRemoved?.Invoke(component);

        return true;
    }

    public Component? AddComponentUnsafe(Component component)
    {
        return AddComponentSafe(component);
    }

    public bool RemoveComponentUnsafe(Component? component)
    {
        return RemoveComponentSafe(component);
    }

    public T? GetComponent<T>() where T : Component
    {
        return Components.OfType<T>().FirstOrDefault();
    }

    public Component? GetComponent(Type type)
    {
        return Components.FirstOrDefault(c => c.GetType() == type);
    }

    public T[] GetComponents<T>() where T : Component
    {
        return Components.OfType<T>().ToArray();
    }

    public static GameObject? FindByName(string name)
    {
        foreach (GameObject obj in SceneManager.ActiveScene.GameObjects)
        {
            if (obj.Name == name) return obj;
        }

        return null;
    }
    
    public static GameObject[] FindAllByName(string name)
    {
        List<GameObject> objs = [];
        foreach (GameObject obj in SceneManager.ActiveScene.GameObjects)
        {
            if (obj.Name == name) objs.Add(obj);
        }

        return objs.ToArray();
    }
    
    public static GameObject? FindFirstByComponent<T>() where T : Component
    {
        foreach (GameObject obj in SceneManager.ActiveScene.GameObjects)
        {
            if (obj.GetComponent<T>() != null)
            {
                return obj;
            }
        }

        return null;
    }

    public static GameObject[] FindAllByComponent<T>() where T : Component
    {
        List<GameObject> objs = [];
        foreach (GameObject obj in SceneManager.ActiveScene.GameObjects)
        {
            if (obj.GetComponent<T>() != null)
            {
                objs.Add(obj);
            }
        }

        return objs.ToArray();
    }

    public static T? FindFirstComponent<T>() where T : Component
    {
        foreach (GameObject obj in SceneManager.ActiveScene.GameObjects)
        {
            T? comp = obj.GetComponent<T>();
            if (comp != null)
            {
                return comp;
            }
        }

        return null;
    }

    public static T[] FindAllComponents<T>() where T : Component
    {
        List<T> components = [];
        foreach (GameObject obj in  SceneManager.ActiveScene.GameObjects)
        {
            components.AddRange(obj.GetComponents<T>());
        }

        return components.ToArray();
    }
}
using MadEngine.Core.SceneManagement;

namespace MadEngine.Core;

public class GameObject
{
    public string Name = "NewGameObject";
    public Guid Id = Guid.NewGuid();
    public Transform Transform;

    public IReadOnlyList<Component> Components => _components;
    private List<Component> _components = [];

    public event Action<Component>? ComponentAdded;
    public event Action<Component>? ComponentRemoved;

    public GameObject()
    {
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
    
    public bool AddComponent<T>() where T : Component
    {
        if (!ComponentRules.CanBeAdded(typeof(T)))
            return false;

        T component = (T)Activator.CreateInstance(typeof(T))!;
        
        _components.Add(component);
        component.AssignGameObject(this);
        
        ComponentAdded?.Invoke(component);
        return true;
    }

    public bool AddComponent(Component component)
    {
        if (!ComponentRules.CanBeAdded(component.GetType()))
            return false;
        
        _components.Add(component);
        component.AssignGameObject(this);
        
        ComponentAdded?.Invoke(component);
        return true;
    }

    public bool RemoveComponent(Component component)
    {
        if (!ComponentRules.CanBeRemoved(component.GetType()))
            return false;

        if (!_components.Remove(component))
            return false;

        ComponentRemoved?.Invoke(component);
        return true;
    }
    
    public bool RemoveComponent<T>() where T : Component
    {
        if (!ComponentRules.CanBeRemoved(typeof(T)))
            return false;

        T component = GetComponent<T>();
        if (component == null)
            return false;
        
        if (!_components.Remove(component))
            return false;

        ComponentRemoved?.Invoke(component);
        return true;
    }

    public T? GetComponent<T>() where T : Component
    {
        return Components.OfType<T>().FirstOrDefault();
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
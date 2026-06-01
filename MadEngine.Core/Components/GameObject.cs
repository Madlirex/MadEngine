using MadEngine.Core.SceneManagement;

namespace MadEngine.Core;

public class GameObject
{
    public string Name = "NewGameObject";
    public Guid Id = Guid.NewGuid();
    public Transform Transform;
    
    public List<Component> Components = [];

    public GameObject(Transform transform)
    {
        Transform = transform;
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
    
    public void AddComponent(Component component)
    {
        Components.Add(component);
        component.AssignGameObject(this);

        if (component is Transform transform)
        {
            Transform = transform;
        }
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
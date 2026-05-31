namespace MadEngine.Core;

public class GameObject
{
    public string Name = "NewGameObject";
    public Transform Transform;
    
    public List<Component> Components = [];

    public GameObject(Transform transform)
    {
        Transform = transform;
        Transform.AssignGameObject(this);
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
}
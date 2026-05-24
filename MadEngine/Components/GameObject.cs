namespace MadEngine;

public class GameObject
{
    public Transform Transform;
    
    public List<Component> Components = [];

    public GameObject(MeshRenderer meshRenderer, Transform transform)
    {
        Transform = transform;
        Transform.AssignGameObject(this);
    }

    public void AddComponent(Component component)
    {
        Components.Add(component);

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
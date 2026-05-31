namespace MadEngine.Core;

public abstract class Component
{
    public GameObject GameObject { get; private set; }

    internal void AssignGameObject(GameObject gameObject)
    {
        GameObject = gameObject;
    }

    public virtual void Awake() {}
    public virtual void Start() {}
    public virtual void Update(float deltaTime) {}

}

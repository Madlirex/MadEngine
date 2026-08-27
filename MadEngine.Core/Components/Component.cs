namespace MadEngine.Core;

public abstract class Component : MadObject
{
    public string ComponentType => GetType().AssemblyQualifiedName!;
    public Component()
    {
        Name = "NewComponent";
    }
    
    public GameObject GameObject => _gameObject;
    private GameObject _gameObject;
    
    internal void AssignGameObject(GameObject gameObject)
    {
        _gameObject = gameObject;
    }

    public virtual void Awake() {}
    public virtual void Start() {}
    public virtual void Update(float deltaTime) {}
    
    public virtual void OnDestroy() {}
    
    public virtual void EditorStart() {}
    public virtual void EditorUpdate(float deltaTime) {}

    public override string ToString()
    {
        return $"{GameObject.Guid}/{Guid}";
    }
}

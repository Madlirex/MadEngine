namespace MadEngine.Core;

public class Component
{
    public GameObject GameObject;

    public void AssignGameObject(GameObject gameObject)
    {
        GameObject = gameObject;
    }
}
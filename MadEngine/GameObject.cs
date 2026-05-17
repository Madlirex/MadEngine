namespace MadEngine;

public class GameObject
{
    public MeshRenderer MeshRenderer;
    public Transform Transform;

    public GameObject(MeshRenderer meshRenderer, Transform transform)
    {
        MeshRenderer = meshRenderer;
        Transform = transform;
        MeshRenderer.GameObject = this;
        Transform.GameObject = this;
    }
}
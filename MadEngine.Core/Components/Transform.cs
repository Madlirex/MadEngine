using OpenTK.Mathematics;

namespace MadEngine.Core;

[CanBeAdded(false)]
[CanBeRemoved(false)]
public class Transform : Component
{
    public Vector3 Position = Vector3.Zero;
    public Vector3 Rotation = Vector3.Zero;
    public Vector3 Scale = Vector3.One;

    public Transform? Parent
    {
        get => _parent;
        set => ChangeParent(value);
    }

    private Transform? _parent;
    
    public IReadOnlyList<Transform> Children => _children;
    private List<Transform> _children = [];

    public Matrix4 GetModuleMatrix()
    {
        Matrix4 translation = Matrix4.CreateTranslation(Position);
        Matrix4 rotation = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Rotation.X)) *
                           Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Rotation.Y)) *
                           Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Rotation.Z));
        Matrix4 scale = Matrix4.CreateScale(Scale);
        return scale * rotation * translation;
    }

    public Matrix4 GetWorldMatrix()
    {
        Matrix4 local = GetModuleMatrix();

        if (Parent == null)
            return local;

        return Parent.GetWorldMatrix() * local;
    }
    
    public Vector3 GetWorldPosition()
    {
        return GetWorldMatrix().ExtractTranslation();
    }

    public Quaternion GetWorldRotation()
    {
        return GetWorldMatrix().ExtractRotation();
    }

    public Vector3 GetWorldScale()
    {
        return GetWorldMatrix().ExtractScale();
    }

    public Transform? GetChild(int index)
    {
        return index <  _children.Count ? _children[index] : throw new IndexOutOfRangeException("Children index out of range.");
    }

    private void AddChild(Transform transform)
    {
        if (_children.Contains(transform))
            throw new InvalidOperationException("Transform already present in children");
        _children.Add(transform);
    }

    private void RemoveChild(Transform transform)
    {
        if (!_children.Remove(transform))
            throw new InvalidOperationException("Child not found");
    }
    
    private bool IsDescendantOf(Transform transform)
    {
        Transform? current = Parent;

        while (current != null)
        {
            if (current == transform)
                return true;

            current = current.Parent;
        }

        return false;
    }

    private void ChangeParent(Transform? parent)
    {
        if (_parent == parent)
            return;

        if (parent == this)
            throw new InvalidOperationException("Cannot parent transform to itself.");

        if (parent != null && parent.IsDescendantOf(this))
            throw new InvalidOperationException("Cannot create cyclic hierarchy.");

        _parent?.RemoveChild(this);

        _parent = parent;

        _parent?.AddChild(this);
    }
}
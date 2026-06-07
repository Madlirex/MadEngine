using OpenTK.Mathematics;

namespace MadEngine.Core;

[CanBeAdded(false)]
[CanBeRemoved(false)]
public class Transform : Component
{
    public Vector3 Position = Vector3.Zero;
    [HideInInspector]
    public Quaternion Rotation = Quaternion.Identity;
    public Vector3 Scale = Vector3.One;

    private Transform? _parent;
    private readonly List<Transform> _children = [];
    private Vector3 _eulerRotation = Vector3.Zero;

    [ShowInInspector]
    public Vector3 EulerRotation
    {
        get => _eulerRotation;
        set
        {
            _eulerRotation = value;
            Rotation = DegreesToRadians(value);
        }
    }
    
    public Transform? Parent
    {
        get => _parent;
        set => SetParent(value);
    }

    public IReadOnlyList<Transform> Children => _children;

    public Quaternion DegreesToRadians(Vector3 value)
    {
        return Quaternion.FromEulerAngles(
            MathHelper.DegreesToRadians(value.X), 
            MathHelper.DegreesToRadians(value.Y), 
            MathHelper.DegreesToRadians(value.Z)
        );
    }
    
    public Matrix4 GetLocalMatrix()
    {
        Matrix4 scale = Matrix4.CreateScale(Scale);
        Matrix4 rotation = Matrix4.CreateFromQuaternion(Rotation);
        Matrix4 translation = Matrix4.CreateTranslation(Position);
        
        return scale * rotation * translation;
    }
    
    public Matrix4 GetWorldMatrix()
    {
        if (_parent == null)
            return GetLocalMatrix();
        
        return GetLocalMatrix() * _parent.GetWorldMatrix();
    }

    public Vector3 GetWorldPosition() => GetWorldMatrix().ExtractTranslation();
    public Quaternion GetWorldRotation() => GetWorldMatrix().ExtractRotation();
    public Vector3 GetWorldScale() => GetWorldMatrix().ExtractScale();
    
    public bool IsDescendantOf(Transform other)
    {
        Transform? current = _parent;
        while (current != null)
        {
            if (current == other) return true;
            current = current._parent;
        }
        return false;
    }

    private void AddChild(Transform child)
    {
        if (!_children.Contains(child)) _children.Add(child);
    }

    private void RemoveChild(Transform child)
    {
        _children.Remove(child);
    }
    
    private void SetParent(Transform? newParent)
    {
        if (_parent == newParent) return;
        if (newParent == this) throw new InvalidOperationException("Cannot parent transform to itself.");
        if (newParent != null && newParent.IsDescendantOf(this)) throw new InvalidOperationException("Cannot create cyclic hierarchy.");
        
        Matrix4 worldMatrixBefore = GetWorldMatrix();
        
        _parent?.RemoveChild(this);
        _parent = newParent;
        _parent?.AddChild(this);
        
        Matrix4 parentWorld = _parent?.GetWorldMatrix() ?? Matrix4.Identity;
        Matrix4 invParentWorld = Matrix4.Invert(parentWorld);
        
        Matrix4 newLocalMatrix = invParentWorld * worldMatrixBefore;

        DecomposeMatrix(newLocalMatrix, out Position, out Rotation, out Scale);
        
        Vector3 eulerRadians = Rotation.ToEulerAngles();
        _eulerRotation = new Vector3(
            MathHelper.RadiansToDegrees(eulerRadians.X),
            MathHelper.RadiansToDegrees(eulerRadians.Y),
            MathHelper.RadiansToDegrees(eulerRadians.Z)
        );
    }
    
    private static void DecomposeMatrix(Matrix4 matrix, out Vector3 position, out Quaternion rotation, out Vector3 scale)
    {
        position = matrix.ExtractTranslation();
        
        scale = new Vector3(
            matrix.Row0.Xyz.Length,
            matrix.Row1.Xyz.Length,
            matrix.Row2.Xyz.Length
        );

        if (MathF.Abs(scale.X - 1.0f) < 0.01f) scale.X = 1.0f;
        if (MathF.Abs(scale.Y - 1.0f) < 0.01f) scale.Y = 1.0f;
        if (MathF.Abs(scale.Z - 1.0f) < 0.01f) scale.Z = 1.0f;
        
        Matrix3 rotMatrix = new Matrix3(matrix);

        if (scale.X > 0.0001f) rotMatrix.Row0 /= scale.X;
        if (scale.Y > 0.0001f) rotMatrix.Row1 /= scale.Y;
        if (scale.Z > 0.0001f) rotMatrix.Row2 /= scale.Z;

        rotation = Quaternion.FromMatrix(rotMatrix);
        rotation.Normalize();
    }
}

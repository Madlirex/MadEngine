using OpenTK.Mathematics;

namespace MadEngine.Core;

public class Transform : Component
{
    public Vector3 Position = Vector3.Zero;
    public Vector3 Rotation = Vector3.Zero;
    public Vector3 Scale = Vector3.One;
    
    public Transform? Parent;

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
}
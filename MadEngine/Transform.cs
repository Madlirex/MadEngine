using OpenTK.Mathematics;

namespace MadEngine;

public class Transform
{
    public Vector3 Position = Vector3.Zero;
    public Vector3 Rotation = Vector3.Zero;
    public Vector3 Scale = Vector3.One;
    
    public GameObject? GameObject;

    public Matrix4 GetModuleMatrix()
    {
        Matrix4 translation = Matrix4.CreateTranslation(Position);
        Matrix4 rotation = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Rotation.X)) *
                           Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Rotation.Y)) *
                           Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Rotation.Z));
        Matrix4 scale = Matrix4.CreateScale(Scale);
        return scale * rotation * translation;
    }
}
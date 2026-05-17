using OpenTK.Mathematics;

namespace MadEngine;

public class Camera
{
    public Transform Transform = new Transform();

    public Vector3 Front = -Vector3.UnitZ;
    public Vector3 Up = Vector3.UnitY;

    public float Speed = 1f;

    public Matrix4 GetViewMatrix()
    {
        return Matrix4.LookAt(Transform.Position, Transform.Position + Front, Up);
    }
}
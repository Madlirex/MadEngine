using OpenTK.Mathematics;

namespace MadEngine;

public class Camera : Component
{
    public float Width;
    public float Height;

    public float DepthNear = 0.01f;
    public float DepthFar = 100f;
    
    public Vector3 Front = -Vector3.UnitZ;
    public Vector3 Up = Vector3.UnitY;
    public Vector3 Right = Vector3.UnitX;
    
    public float Yaw
    {
        get => MathHelper.RadiansToDegrees(_yaw);
        set
        {
            _yaw =  MathHelper.DegreesToRadians(value);
            UpdateVectors();
        }
    }

    public float Pitch 
    { 
        get => MathHelper.RadiansToDegrees(_pitch);
        set
        {
            _pitch = MathHelper.DegreesToRadians(MathHelper.Clamp(value, -89f, 89f));
            UpdateVectors();
        }
    }

    public float Fov
    {
        get => MathHelper.RadiansToDegrees(_fov);
        set => _fov = MathHelper.DegreesToRadians(MathHelper.Clamp(value, 1f, 90f));
    }

    private float _yaw = -MathHelper.PiOver2;
    private float _pitch;
    private float _fov = MathHelper.PiOver2;
    
    public float Speed = 1f;

    public Matrix4 GetViewMatrix()
    {
        Vector3 worldPos = GameObject.Transform.GetWorldPosition();
        return Matrix4.LookAt(GameObject.Transform.Position, GameObject.Transform.Position + Front, Up);
    }

    public Matrix4 GetPerspectiveMatrix()
    {
        return Matrix4.CreatePerspectiveFieldOfView(_fov, Width / Height, DepthNear, DepthFar);
    }

    public void UpdateVectors()
    {
        Front.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
        Front.Y = MathF.Sin(_pitch);
        Front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);

        Front = Vector3.Normalize(Front);
        
        Right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
        Up = Vector3.Normalize(Vector3.Cross(Right, Front));
    }
}
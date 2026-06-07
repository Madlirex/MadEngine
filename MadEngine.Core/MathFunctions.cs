using OpenTK.Mathematics;
using NVector3 = System.Numerics.Vector3;
using NVector4 = System.Numerics.Vector4;

namespace MadEngine.Core;

public static class MathFunctions
{
    public static Vector3 ToOtk3(NVector3 vector3)
    {
        return new Vector3(vector3.X, vector3.Y, vector3.Z);
    }

    public static Quaternion ToQuaternion(NVector3 vector3)
    {
        return new Quaternion(vector3.X, vector3.Y, vector3.Z);
    }

    public static NVector3 ToNumerics3(Vector3 vector3)
    {
        return new NVector3(vector3.X, vector3.Y, vector3.Z);
    }

    public static NVector3 ToNumerics3(Quaternion quaternion)
    {
        return new NVector3(quaternion.X, quaternion.Y, quaternion.Z);
    }
    
    public static Vector4 ToOtk4(NVector4 vector4)
    {
        return new Vector4(vector4.X, vector4.Y, vector4.Z, vector4.W);
    }

    public static NVector4 ToNumerics4(Vector4 vector4)
    {
        return new NVector4(vector4.X, vector4.Y, vector4.Z, vector4.W);
    }
}
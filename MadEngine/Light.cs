using OpenTK.Mathematics;

namespace MadEngine;

public class Light(MeshRenderer meshRenderer, Transform transform) : GameObject(meshRenderer, transform)
{
    public Vector4 AmbientColor = Vector4.One * 0.1f;
    public Vector4 DiffuseColor = Vector4.One;
    public Vector4 SpecularColor = Vector4.One * 0.5f;
}
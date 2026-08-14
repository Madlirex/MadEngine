using OpenTK.Mathematics;

namespace MadEngine.Core;

public abstract class Renderer : Component
{
    public abstract void Draw(Matrix4 view, Matrix4 projection);
}
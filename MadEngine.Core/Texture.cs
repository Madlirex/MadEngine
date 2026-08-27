using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace MadEngine.Core;

public abstract class Texture : Asset
{
    [DoNotSave] public int Handle { get; protected set; }
    [ShowInInspector] public string FilePath { get; protected set; } = "";

    public override string Name { get; set; } = "NewTexture";
    
    public abstract TextureTarget Target { get; }

    public Texture()
    {
        Handle = GL.GenTexture();
    }

    public void Bind(TextureUnit unit = TextureUnit.Texture0)
    {
        GL.ActiveTexture(unit);
        GL.BindTexture(Target, Handle);
    }
    
    protected override void OnDispose(bool disposing)
    {
        if (Handle != 0)
        {
            GL.DeleteTexture(Handle);
            Handle = 0;
        }
        base.OnDispose(disposing);
    }
}
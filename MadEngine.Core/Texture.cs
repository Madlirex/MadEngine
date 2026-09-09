using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace MadEngine.Core;

public abstract class Texture : Asset, IStateUpdateable
{
    [DoNotSave] public int Handle { get; protected set; }

    [ShowInInspector] public ImageAsset? Image
    {
        get => _image;
        set
        {
            _image = value;
            UpdateState();
        }
    }

    private ImageAsset? _image;

    protected override string NameInternal { get; set; } = "NewTexture";
    protected  override string ExtensionInternal { get; set; } = ".tex";
    
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

    public abstract void UpdateState();
}
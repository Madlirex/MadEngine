using MadEngine.Core;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace MadEngine;

public class Texture2D : Texture, IStateUpdateable
{
    public override string Extension => ".tex";
    public override TextureTarget Target => TextureTarget.Texture2D;

    public Texture2D() {}
    
    public Texture2D(string path)
    {
        FilePath = path;
        LoadFromFile();
    }

    private void LoadFromFile()
    {
        if (!File.Exists(FilePath)) return;
        
        GL.BindTexture(Target, Handle);
        
        StbImage.stbi_set_flip_vertically_on_load(1);
        using Stream stream = File.OpenRead(FilePath);
        ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
        
        GL.TexImage2D(Target, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
        
        GL.TexParameter(Target, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        GL.TexParameter(Target, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(Target, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(Target, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        
        GL.BindTexture(Target, 0);
    }

    public void UpdateState()
    {
        LoadFromFile();
    }
}
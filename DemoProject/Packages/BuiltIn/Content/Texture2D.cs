using MadEngine.Core;
using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

namespace MadEngine;

public class Texture2D : Texture, IStateUpdateable
{
    public override TextureTarget Target => TextureTarget.Texture2D;

    public Texture2D() {}
    
    public Texture2D(string path)
    {
        Image = new ImageAsset()
        {
            Extension =  Path.GetExtension(path),
            FullDir = Path.GetDirectoryName(path) ?? "",
            Name = Path.GetFileNameWithoutExtension(path)
        };
        LoadFromFile();
    }

    private void LoadFromFile()
    {
        if (Image == null) return;
        Console.WriteLine(Image.Path);
        if (!File.Exists(Image.Path)) return;
        Console.WriteLine(Image.Path);
        
        GL.BindTexture(Target, Handle);
        
        StbImage.stbi_set_flip_vertically_on_load(1);
        using Stream stream = File.OpenRead(Image.Path);
        ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
        
        GL.TexImage2D(Target, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
        
        GL.TexParameter(Target, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
        GL.TexParameter(Target, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(Target, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(Target, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
        
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        Console.WriteLine("loaded");
        GL.BindTexture(Target, 0);
    }

    public override void UpdateState()
    {
        LoadFromFile();
    }
}
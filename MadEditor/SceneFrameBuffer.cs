using OpenTK.Graphics.OpenGL4;
 
namespace MadEditor;
 

public class SceneFramebuffer : IDisposable
{
    public int ColorTexture { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
 
    private int _fbo;
    private int _depthRbo;
 
    public SceneFramebuffer(int width, int height)
    {
        Create(width, height);
    }
    
    public void Bind()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);
        GL.Viewport(0, 0, Width, Height);
    }
    
    public static void Unbind()
    {
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }
    
    public void Resize(int width, int height)
    {
        if (width == Width && height == Height) return;
        Delete();
        Create(width, height);
    }
 
    private void Create(int width, int height)
    {
        Width  = Math.Max(1, width);
        Height = Math.Max(1, height);
        
        ColorTexture = GL.GenTexture();
        GL.BindTexture(TextureTarget.Texture2D, ColorTexture);
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, Width, Height, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.BindTexture(TextureTarget.Texture2D, 0);
        
        _depthRbo = GL.GenRenderbuffer();
        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, _depthRbo);
        GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer,
                               RenderbufferStorage.Depth24Stencil8, Width, Height);
        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
        
        _fbo = GL.GenFramebuffer();
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, _fbo);
        GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                                FramebufferAttachment.ColorAttachment0,
                                TextureTarget.Texture2D, ColorTexture, 0);
        GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer,
                                   FramebufferAttachment.DepthStencilAttachment,
                                   RenderbufferTarget.Renderbuffer, _depthRbo);
 
        var status = GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer);
        if (status != FramebufferErrorCode.FramebufferComplete)
            throw new Exception($"Framebuffer incomplete: {status}");
 
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
    }
 
    private void Delete()
    {
        if (_fbo != 0) { GL.DeleteFramebuffer(_fbo); _fbo = 0; }
        if (ColorTexture != 0) { GL.DeleteTexture(ColorTexture); ColorTexture = 0; }
        if (_depthRbo != 0) { GL.DeleteRenderbuffer(_depthRbo); _depthRbo = 0; }
    }
 
    public void Dispose() => Delete();
}
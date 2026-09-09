using System.Numerics;
using MadEngine.Core;

namespace MadEditor;

public class ViewportContext : IDisposable
{
    public bool EditMode { get; internal set; }
    
    public string Id { get; }
    public GameObject CameraObject
    {
        get
        {
            if (_camera != null) return _camera;
            _camera = new GameObject();
            _cameraComponent = new Camera();
            _camera.AddComponentUnsafe(_cameraComponent);
            return _camera;
        }
        internal set  => _camera = value;
    }

    private GameObject? _camera;

    public Camera CameraComponent
    {
        get
        {
            if(_cameraComponent != null) return _cameraComponent;
            _camera = null;
            return CameraObject.GetComponent<Camera>()!;
        }
        internal set => _cameraComponent = value;
    }

    private Camera? _cameraComponent;
    public SceneFramebuffer Framebuffer { get; }
    public Vector2 Size { get; set; } = Vector2.Zero;

    public ViewportContext(string id, int initialWidth, int initialHeight)
    {
        Id = id;
        
        _camera = new GameObject { Name = $"EditorCamera_{id}" };
        _cameraComponent = new Camera();
        _camera.AddComponentUnsafe(_cameraComponent);

        _cameraComponent.Width = initialWidth;
        _cameraComponent.Height = initialHeight;
        Framebuffer = new SceneFramebuffer(initialWidth, initialHeight);
    }

    public void Dispose()
    {
        Framebuffer.Dispose();
        _camera?.Destroy();
    }
}
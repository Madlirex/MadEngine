using System.Numerics;
using MadEngine.Core;
using OpenTK.Windowing.Desktop;

namespace MadEditor;

public class EditorUIContext
{
    public GameObject? Selected;
    public required GameObject CameraObject;
    public required SceneFramebuffer SceneFbo;
    public Vector2 ViewportSize;
    public GameWindow Window = null!;
}
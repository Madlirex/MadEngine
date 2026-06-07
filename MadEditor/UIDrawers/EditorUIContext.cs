using System.Numerics;
using MadEngine.Core;
using OpenTK.Windowing.Desktop;

namespace MadEditor;

public class EditorUIContext
{
    public GameObject? Selected;
    public GameObject? RightClicked;
    
    public required GameObject CameraObject;
    public required SceneFramebuffer SceneFbo;
    public Vector2 ViewportSize;
    public GameWindow Window = null!;

    private List<IEditorCommand> _commands = [];
    
    public void EnqueueCommand(IEditorCommand command)
    {
        _commands.Add(command);
    }

    public void DequeueCommand(IEditorCommand command)
    {
        _commands.Remove(command);
    }

    public void ExecuteCommands()
    {
        foreach (var command in _commands)
        {
            command.Execute();
        }
    }
    
    public void ClearCommands()
    {
        _commands.Clear();
    }
}
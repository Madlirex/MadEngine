using System.Numerics;
using MadEngine.Core;
using OpenTK.Windowing.Desktop;

namespace MadEditor;

public class EditorUIContext
{
    
    public MadObject? Selected;
    public MadObject? RightClicked;
    
    public GameObject CameraObject = new();
    public SceneFramebuffer SceneFbo = new(600, 600);
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
            command.Execute(this);
        }
    }
    
    public void ClearCommands()
    {
        _commands.Clear();
    }
}
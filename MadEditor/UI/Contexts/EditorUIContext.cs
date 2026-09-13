using System.Numerics;
using MadEngine.Core;
using OpenTK.Windowing.Desktop;

namespace MadEditor;

public class EditorUIContext
{
    public Engine Engine;
    
    public MadObject? Selected;
    public MadObject? RightClicked;
    public GameWindow Window = null!;
    public bool IsPlaying { get; internal set; }
    
    private readonly Dictionary<string, ViewportContext> _activeViewports = new();
    
    public ViewportContext? ActiveViewport { get; set; }

    private Queue<IEditorCommand> _commands = [];

    public EditorUIContext(Engine engine)
    {
        Engine = engine;
    }
    
    public ViewportContext GetOrCreateViewport(string panelTitle)
    {
        if (_activeViewports.TryGetValue(panelTitle, out var context)) return context;
        context = new ViewportContext(panelTitle, 800, 600);
        _activeViewports[panelTitle] = context;

        ActiveViewport = context;
        return context;
    }

    public ViewportContext[] GetAllViewports()
    {
        return _activeViewports.Values.ToArray();
    }
    
    public void EnqueueCommand(IEditorCommand command)
    {
        _commands.Enqueue(command);
    }

    public void DequeueCommand()
    {
        _commands.Dequeue();
    }

    public void ExecuteCommands()
    {
        while (_commands.Count > 0)
        {
            var command = _commands.Dequeue();

            if (command is IPopupCommand popupCommand) 
                popupCommand.Execute(RightClicked!);
            else 
                command.Execute(this);
        }
    }
    
    public void ClearCommands()
    {
        _commands.Clear();
    }
}
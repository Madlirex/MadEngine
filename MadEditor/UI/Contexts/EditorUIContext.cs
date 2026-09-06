using System.Numerics;
using MadEngine.Core;
using OpenTK.Windowing.Desktop;

namespace MadEditor;

public class EditorUIContext
{
    public MadObject? Selected;
    public MadObject? RightClicked;
    public GameWindow Window = null!;
    
    private readonly Dictionary<string, ViewportContext> _activeViewports = new();
    
    public ViewportContext? ActiveViewport { get; set; }

    private List<IEditorCommand> _commands = [];
    
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
            if(command is IPopupCommand popupCommand) popupCommand.Execute(RightClicked!);
            else command.Execute(this);
        }
    }
    
    public void ClearCommands()
    {
        _commands.Clear();
    }
}
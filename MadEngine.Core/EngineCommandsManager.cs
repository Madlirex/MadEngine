namespace MadEngine.Core;

public abstract class EngineCommand(object target)
{
    public object Target = target;

    public abstract void Execute(object target);
}

public static class EngineCommandsManager
{
    private static readonly List<EngineCommand> _commands = [];

    public static void Enqueue(EngineCommand command)
    {
        _commands.Add(command);
    }

    public static void Dequeue(EngineCommand command)
    {
        _commands.Remove(command);
    }

    public static void ExecuteAll()
    {
        foreach (var command in _commands)
        {
            command.Execute(command.Target);
        }
        _commands.Clear();
    }
}
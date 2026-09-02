namespace MadEngine.Core;

public abstract class EngineCommand
{
    public abstract void Execute();
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
            command.Execute();
        }
        _commands.Clear();
    }
}
namespace MadEngine.Core;

public abstract class EngineCommand
{
    public abstract void Execute();
}

public static class EngineCommandsManager
{
    private static readonly Queue<EngineCommand> Commands = [];

    public static void Enqueue(EngineCommand command)
    {
        Commands.Enqueue(command);
    }

    public static void Dequeue()
    {
        Commands.Dequeue();
    }

    public static void ExecuteAll()
    {
        while (Commands.Count > 0)
        {
            var command = Commands.Dequeue();

            command.Execute();
        }
        Commands.Clear();
    }
}
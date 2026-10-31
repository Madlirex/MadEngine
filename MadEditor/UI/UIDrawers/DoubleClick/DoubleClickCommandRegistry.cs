using MadEngine.Core;

namespace MadEditor.DoubleClick;

public static class DoubleClickCommandRegistry
{
    private static readonly DoubleClickCommandEngine Instance = RegistryBootstrapper.Get<DoubleClickCommandEngine>();

    public static void DiscoverCommands() => Instance.DiscoverCommands();
    public static IDoubleClickCommand? GetCommand(Type type) => Instance.GetCommand(type);
    public static DoubleClickCommand<T>? GetCommand<T>() => Instance.GetCommand<T>();
}

internal class DoubleClickCommandEngine : Registry
{
    internal readonly List<IDoubleClickCommand> Commands = [];
    
    public override void Initialize()
    {
        DiscoverCommands();
    }

    public void DiscoverCommands()
    {
        Commands.Clear();
        
        var types = AssemblyProvider.GetTypesImplementing(typeof(IDoubleClickCommand));
        foreach (var type in types)
        {
            Console.WriteLine(type);
            var instance = Activator.CreateInstance(type);
            if (instance is IDoubleClickCommand inst) Commands.Add(inst);
        }
    }

    public IDoubleClickCommand? GetCommand(Type type)
    {
        return Commands.FirstOrDefault(c => c.TargetType == type);
    }

    public DoubleClickCommand<T>? GetCommand<T>()
    {
        return GetCommand(typeof(T)) as DoubleClickCommand<T>;
    }
}
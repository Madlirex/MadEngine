namespace MadEngine.Runtime;

class Program
{
    static void Main(string[] args)
    {
        using RuntimeWindow game = RuntimeWindow.Create(800, 600, "Mad Engine");
        game.Run();
    }
}
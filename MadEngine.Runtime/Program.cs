namespace MadEngine.Runtime;

class Program
{
    static void Main(string[] args)
    {
        using (RuntimeWindow game = new RuntimeWindow(800, 600, "Mad Engine"))
        {
            game.Run();
        }
    }
}
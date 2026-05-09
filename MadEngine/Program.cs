namespace MadEngine;

class Program
{
    static void Main(string[] args)
    {
        using (Game game = new Game(800, 600, "Mad Engine"))
        {
            game.Run();
        }
    }
}
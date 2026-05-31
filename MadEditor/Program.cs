namespace MadEditor;

class Program
{
    static void Main(string[] args)
    {
        using (EditorWindow game = new EditorWindow(1200, 800, "Mad Engine"))
        {
            game.Run();
        }
    }
}
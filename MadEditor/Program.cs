namespace MadEditor;

class Program
{
    static void Main(string[] args)
    {
        using (EditorWindow game = new EditorWindow(1920, 1080, "Mad Engine"))
        {
            game.Run();
        }
    }
}
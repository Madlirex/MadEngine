using NativeFileDialogSharp;

namespace MadEditor;

class Program
{
    static void Main(string[] args)
    {
        string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "engine-log.txt");

        using DualWriter dualWriter = new DualWriter(Console.Out, logPath);
        Console.SetOut(dualWriter);
            
        Console.SetError(dualWriter); 

        Console.WriteLine($"--- Engine Session Started: {DateTime.Now} ---");

        try
        {
            while (true)
            {
                DialogResult result = Dialog.FolderPicker();

                if (result.IsCancelled) return;
                if (!result.IsOk) continue;
                AssetManager.SetProjectPath(result.Path); 
                break;
            }
                
            Diagnostics.EngineCrashHook.Initialize();

            using EditorWindow game = new EditorWindow(1200, 800, "Mad Engine");
            game.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine("\n[FATAL CRASH DETECTED]");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine($"Stack Trace:\n{ex.StackTrace}");
                
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                Console.WriteLine($"Inner Stack Trace:\n{ex.InnerException.StackTrace}");
            }

            Console.WriteLine("\nPress any key to close...");
            Console.ReadKey();
        }
    }
}


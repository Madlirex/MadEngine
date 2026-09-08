using System.Runtime.InteropServices;
using NativeFileDialogSharp;

namespace MadEditor;

class Program
{
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);
    
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

            string title = "MadEditor - Fatal Crash";
            string message = $"The engine encountered an unhandled exception and must close.\n\n" +
                             $"Error: {ex.Message}\n\n" +
                             $"A full stack trace has been saved to 'engine-log.txt'.";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                MessageBox(IntPtr.Zero, message, title, 0x00000010);
            }
        }
        finally
        {
            Console.WriteLine($"--- Engine Session Ended: {DateTime.Now} ---");
        }
    }
}


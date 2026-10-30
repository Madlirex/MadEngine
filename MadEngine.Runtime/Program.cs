using System.Reflection;
using System.Runtime.InteropServices;
using MadEngine.Core;

namespace MadEngine.Runtime;

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
            AssemblyProvider.InitializeAssemblies();
            
            Application.Directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
            RegistryBootstrapper.InitializeAll();
            ProjectSettingsManager.LoadAllSettings();

            using RuntimeWindow game =
                RuntimeWindow.Create(1200, 800, ProjectSettingsRegistry.GetSettings<ProjectInfo>()?.Name ?? Assembly.GetEntryAssembly()?.GetName().Name ?? "MadGame");
            Console.WriteLine(Assembly.GetEntryAssembly()?.GetName().Name);
            Console.WriteLine(game.Title);
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
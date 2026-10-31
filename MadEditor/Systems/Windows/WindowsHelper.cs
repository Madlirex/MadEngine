using System.ComponentModel;
using System.Diagnostics;

namespace MadEditor;

public static class WindowsHelper
{
    public static void OpenFile(string file)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = file,
                UseShellExecute = true
            });
        }
        catch (Win32Exception)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "rundll32.exe",
                Arguments = $"shell32.dll,OpenAs_RunDLL {file}",
                UseShellExecute = true
            });
        }
    }
}
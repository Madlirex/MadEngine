using MadEngine.Core;

namespace MadEditor.Diagnostics;

public static class EngineCrashHook
{
    private static bool _isInitialized;

    public static void Initialize()
    {
        if (_isInitialized) return;
        
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        
        TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;

        _isInitialized = true;
    }

    private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception ex)
        {
            FormatAndLogException(ex);
        }
        else
        {
            Debug.LogError($"An unidentified runtime error object occurred: {e.ExceptionObject}");
        }
    }

    private static void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        e.SetObserved(); 
        
        if (e.Exception is Exception ex)
        {
            FormatAndLogException(ex);
        }
    }

    private static void FormatAndLogException(Exception ex)
    {
        string message = ex.Message;
        string stackTrace = ex.StackTrace ?? Environment.StackTrace;

        Exception? inner = ex.InnerException;
        while (inner != null)
        {
            message += $"\n ---> [Inner Exception]: {inner.Message}";
            inner = inner.InnerException;
        }

        Debug.LogError($"{ex.GetType().Name}: {message}\n{stackTrace}");
    }
}

namespace MadEngine.Core;

public enum LogType
{
    Info,
    Warning,
    Error
};

public class LogEntry
{
    public string Message { get; init; }
    public string StackTrace { get; init; }
    public LogType Type { get; init; }
    public DateTime Timestamp { get; init; }
    public int Count { get; set; }

    public LogEntry(string message, string stackTrace, LogType type)
    {
        Message = message;
        StackTrace = stackTrace;
        Type = type;
        Timestamp = DateTime.Now;
        Count = 1;
    }
}
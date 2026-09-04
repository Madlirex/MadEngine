using System;
using System.Collections.Generic;

namespace MadEngine.Core;

public static class Debug
{
    private static readonly List<LogEntry> Logs = [];
    private static readonly Lock Lock = new();
    
    public static int MaxLogCount { get; set; } = 2000;
    
    public static event Action? OnLogAdded;

    public static void Log(string message) => AddEntry(message, LogType.Info);
    public static void LogWarning(string message) => AddEntry(message, LogType.Warning);
    public static void LogError(string message) => AddEntry(message, LogType.Error);

    private static void AddEntry(string message, LogType type)
    {
        lock (Lock)
        {
            if (Logs.Count > 0 && Logs[^1].Message == message && Logs[^1].Type == type)
            {
                var last = Logs[^1];
                last.Count++;
                Logs[^1] = last;
            }
            else
            {
                Logs.Add(new LogEntry(message, Environment.StackTrace, type));
            }
            
            if (Logs.Count > MaxLogCount)
            {
                Logs.RemoveAt(0);
            }
        }
        
        OnLogAdded?.Invoke();
    }
    
    public static List<LogEntry> GetReadOnlyLogs()
    {
        lock (Lock)
        {
            return [..Logs];
        }
    }

    public static void Clear()
    {
        lock (Lock)
        {
            Logs.Clear();
        }
        OnLogAdded?.Invoke();
    }
}


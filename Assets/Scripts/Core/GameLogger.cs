using UnityEngine;

/// <summary>
/// Simple static logger utility for categorized and level-based logging in Time Riders.
/// </summary>
public static class GameLogger
{
    public enum LogLevel { Debug, Info, Warning, Error }

    // Set the minimum log level. Change this to filter logs.
    public static LogLevel MinimumLevel = LogLevel.Debug;

    /// <summary>
    /// Log a message with a category and level.
    /// </summary>
    /// <param name="category">Category or class name</param>
    /// <param name="level">Log level</param>
    /// <param name="message">The log message</param>
    public static void Log(string category, LogLevel level, string message)
    {
        if (level < MinimumLevel) return;
        string prefix = $"[{category}] [{level}] ";
        switch (level)
        {
            case LogLevel.Debug:
            case LogLevel.Info:
                Debug.Log(prefix + message);
                break;
            case LogLevel.Warning:
                Debug.LogWarning(prefix + message);
                break;
            case LogLevel.Error:
                Debug.LogError(prefix + message);
                break;
        }
    }

    /// <summary>
    /// Log a debug message.
    /// </summary>
    public static void Debug(string category, string message) => Log(category, LogLevel.Debug, message);

    /// <summary>
    /// Log an info message.
    /// </summary>
    public static void Info(string category, string message) => Log(category, LogLevel.Info, message);

    /// <summary>
    /// Log a warning message.
    /// </summary>
    public static void Warning(string category, string message) => Log(category, LogLevel.Warning, message);

    /// <summary>
    /// Log an error message.
    /// </summary>
    public static void Error(string category, string message) => Log(category, LogLevel.Error, message);
}

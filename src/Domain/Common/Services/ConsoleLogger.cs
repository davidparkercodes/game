using System;

namespace Game.Domain.Common.Services;

public enum LogLevel
{
    Error,
    Warning,
    Information,
    Debug
}

public class ConsoleLogger : ILogger
{
    private readonly string _prefix;
    private readonly LogLevel _logLevel;

    public ConsoleLogger(string prefix = "", LogLevel logLevel = LogLevel.Information)
    {
        _prefix = string.IsNullOrEmpty(prefix) ? "" : $"{prefix} ";
        _logLevel = logLevel;
    }

    public void LogInformation(string message)
    {
        if (_logLevel >= LogLevel.Information)
        {
            Console.WriteLine($"{_prefix}{message}");
        }
    }

    public void LogWarning(string message)
    {
        if (_logLevel >= LogLevel.Warning)
        {
            Console.WriteLine($"{_prefix}⚠️ WARNING: {message}");
        }
    }

    public void LogError(string message)
    {
        if (_logLevel >= LogLevel.Error)
        {
            Console.WriteLine($"{_prefix}❌ ERROR: {message}");
        }
    }

    public void LogDebug(string message)
    {
        if (_logLevel >= LogLevel.Debug)
        {
            Console.WriteLine($"{_prefix}🐛 DEBUG: {message}");
        }
    }
}

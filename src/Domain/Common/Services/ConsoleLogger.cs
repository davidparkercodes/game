using System;

namespace Game.Domain.Common.Services;

public class ConsoleLogger : ILogger
{
    private readonly string _prefix;

    public ConsoleLogger(string prefix = "")
    {
        _prefix = string.IsNullOrEmpty(prefix) ? "" : $"{prefix} ";
    }

    public void LogInformation(string message)
    {
        Console.WriteLine($"{_prefix}{message}");
    }

    public void LogWarning(string message)
    {
        Console.WriteLine($"{_prefix}⚠️ WARNING: {message}");
    }

    public void LogError(string message)
    {
        Console.WriteLine($"{_prefix}❌ ERROR: {message}");
    }

    public void LogDebug(string message)
    {
        Console.WriteLine($"{_prefix}🐛 DEBUG: {message}");
    }
}

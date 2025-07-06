using System;
using System.IO;
using FluentAssertions;
using Game.Domain.Common.Services;
using Xunit;

namespace Game.Tests.Domain.Common.Services;

public class ConsoleLoggerTests
{
    private static string CaptureConsoleOutput(Action action)
    {
        var originalOut = Console.Out;
        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);
        
        try
        {
            action();
            return stringWriter.ToString().Trim();
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    [Fact]
    public void Constructor_WithDefaultParameters_ShouldCreateLogger()
    {
        var logger = new ConsoleLogger();

        logger.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithPrefix_ShouldCreateLoggerWithPrefix()
    {
        var prefix = "TEST";
        var logger = new ConsoleLogger(prefix);

        logger.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithLogLevel_ShouldCreateLoggerWithLogLevel()
    {
        var logger = new ConsoleLogger("TEST", LogLevel.Debug);

        logger.Should().NotBeNull();
    }

    [Fact]
    public void LogInformation_WithDefaultLogLevel_ShouldOutputMessage()
    {
        var logger = new ConsoleLogger();
        var message = "Test information message";

        var output = CaptureConsoleOutput(() => logger.LogInformation(message));

        output.Should().Be(message);
    }

    [Fact]
    public void LogInformation_WithPrefix_ShouldOutputMessageWithPrefix()
    {
        var prefix = "APP";
        var logger = new ConsoleLogger(prefix);
        var message = "Test information message";

        var output = CaptureConsoleOutput(() => logger.LogInformation(message));

        output.Should().Be($"{prefix} {message}");
    }

    [Fact]
    public void LogInformation_WithEmptyPrefix_ShouldOutputMessageWithoutPrefix()
    {
        var logger = new ConsoleLogger("");
        var message = "Test information message";

        var output = CaptureConsoleOutput(() => logger.LogInformation(message));

        output.Should().Be(message);
    }

    [Fact]
    public void LogInformation_WithNullPrefix_ShouldOutputMessageWithoutPrefix()
    {
        var logger = new ConsoleLogger(prefix: null!);
        var message = "Test information message";

        var output = CaptureConsoleOutput(() => logger.LogInformation(message));

        output.Should().Be(message);
    }

    [Fact]
    public void LogWarning_WithDefaultLogLevel_ShouldOutputWarningMessage()
    {
        var logger = new ConsoleLogger();
        var message = "Test warning message";

        var output = CaptureConsoleOutput(() => logger.LogWarning(message));

        output.Should().Be($"⚠️ WARNING: {message}");
    }

    [Fact]
    public void LogWarning_WithPrefix_ShouldOutputWarningMessageWithPrefix()
    {
        var prefix = "APP";
        var logger = new ConsoleLogger(prefix);
        var message = "Test warning message";

        var output = CaptureConsoleOutput(() => logger.LogWarning(message));

        output.Should().Be($"{prefix} ⚠️ WARNING: {message}");
    }

    [Fact]
    public void LogError_WithDefaultLogLevel_ShouldOutputErrorMessage()
    {
        var logger = new ConsoleLogger();
        var message = "Test error message";

        var output = CaptureConsoleOutput(() => logger.LogError(message));

        output.Should().Be($"❌ ERROR: {message}");
    }

    [Fact]
    public void LogError_WithPrefix_ShouldOutputErrorMessageWithPrefix()
    {
        var prefix = "APP";
        var logger = new ConsoleLogger(prefix);
        var message = "Test error message";

        var output = CaptureConsoleOutput(() => logger.LogError(message));

        output.Should().Be($"{prefix} ❌ ERROR: {message}");
    }

    [Fact]
    public void LogDebug_WithDebugLogLevel_ShouldOutputDebugMessage()
    {
        var logger = new ConsoleLogger("", LogLevel.Debug);
        var message = "Test debug message";

        var output = CaptureConsoleOutput(() => logger.LogDebug(message));

        output.Should().Be($"🐛 DEBUG: {message}");
    }

    [Fact]
    public void LogDebug_WithPrefix_ShouldOutputDebugMessageWithPrefix()
    {
        var prefix = "DEBUG";
        var logger = new ConsoleLogger(prefix, LogLevel.Debug);
        var message = "Test debug message";

        var output = CaptureConsoleOutput(() => logger.LogDebug(message));

        output.Should().Be($"{prefix} 🐛 DEBUG: {message}");
    }

    [Theory]
    [InlineData(LogLevel.Error)]
    [InlineData(LogLevel.Warning)]
    [InlineData(LogLevel.Information)]
    [InlineData(LogLevel.Debug)]
    public void LogError_WithAnyLogLevel_ShouldAlwaysOutput(LogLevel logLevel)
    {
        var logger = new ConsoleLogger("", logLevel);
        var message = "Test error message";

        var output = CaptureConsoleOutput(() => logger.LogError(message));

        output.Should().Be($"❌ ERROR: {message}");
    }

    [Theory]
    [InlineData(LogLevel.Warning)]
    [InlineData(LogLevel.Information)]
    [InlineData(LogLevel.Debug)]
    public void LogWarning_WithSufficientLogLevel_ShouldOutput(LogLevel logLevel)
    {
        var logger = new ConsoleLogger("", logLevel);
        var message = "Test warning message";

        var output = CaptureConsoleOutput(() => logger.LogWarning(message));

        output.Should().Be($"⚠️ WARNING: {message}");
    }

    [Fact]
    public void LogWarning_WithErrorLogLevel_ShouldNotOutput()
    {
        var logger = new ConsoleLogger("", LogLevel.Error);
        var message = "Test warning message";

        var output = CaptureConsoleOutput(() => logger.LogWarning(message));

        output.Should().BeEmpty();
    }

    [Theory]
    [InlineData(LogLevel.Information)]
    [InlineData(LogLevel.Debug)]
    public void LogInformation_WithSufficientLogLevel_ShouldOutput(LogLevel logLevel)
    {
        var logger = new ConsoleLogger("", logLevel);
        var message = "Test information message";

        var output = CaptureConsoleOutput(() => logger.LogInformation(message));

        output.Should().Be(message);
    }

    [Theory]
    [InlineData(LogLevel.Error)]
    [InlineData(LogLevel.Warning)]
    public void LogInformation_WithInsufficientLogLevel_ShouldNotOutput(LogLevel logLevel)
    {
        var logger = new ConsoleLogger("", logLevel);
        var message = "Test information message";

        var output = CaptureConsoleOutput(() => logger.LogInformation(message));

        output.Should().BeEmpty();
    }

    [Fact]
    public void LogDebug_WithDebugLogLevel_ShouldOutput()
    {
        var logger = new ConsoleLogger("", LogLevel.Debug);
        var message = "Test debug message";

        var output = CaptureConsoleOutput(() => logger.LogDebug(message));

        output.Should().Be($"🐛 DEBUG: {message}");
    }

    [Theory]
    [InlineData(LogLevel.Error)]
    [InlineData(LogLevel.Warning)]
    [InlineData(LogLevel.Information)]
    public void LogDebug_WithInsufficientLogLevel_ShouldNotOutput(LogLevel logLevel)
    {
        var logger = new ConsoleLogger("", logLevel);
        var message = "Test debug message";

        var output = CaptureConsoleOutput(() => logger.LogDebug(message));

        output.Should().BeEmpty();
    }

    [Fact]
    public void LogInformation_WithNullMessage_ShouldHandleGracefully()
    {
        var logger = new ConsoleLogger();

        var output = CaptureConsoleOutput(() => logger.LogInformation(null!));

        output.Should().BeEmpty();
    }

    [Fact]
    public void LogWarning_WithNullMessage_ShouldHandleGracefully()
    {
        var logger = new ConsoleLogger();

        var output = CaptureConsoleOutput(() => logger.LogWarning(null!));

        output.Should().Be("⚠️ WARNING:");
    }

    [Fact]
    public void LogError_WithNullMessage_ShouldHandleGracefully()
    {
        var logger = new ConsoleLogger();

        var output = CaptureConsoleOutput(() => logger.LogError(null!));

        output.Should().Be("❌ ERROR:");
    }

    [Fact]
    public void LogDebug_WithNullMessage_ShouldHandleGracefully()
    {
        var logger = new ConsoleLogger("", LogLevel.Debug);

        var output = CaptureConsoleOutput(() => logger.LogDebug(null!));

        output.Should().Be("🐛 DEBUG:");
    }

    [Fact]
    public void LogInformation_WithEmptyMessage_ShouldOutputEmptyMessage()
    {
        var logger = new ConsoleLogger();

        var output = CaptureConsoleOutput(() => logger.LogInformation(""));

        output.Should().BeEmpty();
    }

    [Fact]
    public void LogWarning_WithEmptyMessage_ShouldOutputWarningPrefix()
    {
        var logger = new ConsoleLogger();

        var output = CaptureConsoleOutput(() => logger.LogWarning(""));

        output.Should().Be("⚠️ WARNING:");
    }

    [Fact]
    public void LogError_WithEmptyMessage_ShouldOutputErrorPrefix()
    {
        var logger = new ConsoleLogger();

        var output = CaptureConsoleOutput(() => logger.LogError(""));

        output.Should().Be("❌ ERROR:");
    }

    [Fact]
    public void LogDebug_WithEmptyMessage_ShouldOutputDebugPrefix()
    {
        var logger = new ConsoleLogger("", LogLevel.Debug);

        var output = CaptureConsoleOutput(() => logger.LogDebug(""));

        output.Should().Be("🐛 DEBUG:");
    }

    [Fact]
    public void MultipleLogCalls_ShouldAllWorkCorrectly()
    {
        var logger = new ConsoleLogger("APP", LogLevel.Debug);
        var outputs = new string[4];

        outputs[0] = CaptureConsoleOutput(() => logger.LogError("Error message"));
        outputs[1] = CaptureConsoleOutput(() => logger.LogWarning("Warning message"));
        outputs[2] = CaptureConsoleOutput(() => logger.LogInformation("Info message"));
        outputs[3] = CaptureConsoleOutput(() => logger.LogDebug("Debug message"));

        outputs[0].Should().Be("APP ❌ ERROR: Error message");
        outputs[1].Should().Be("APP ⚠️ WARNING: Warning message");
        outputs[2].Should().Be("APP Info message");
        outputs[3].Should().Be("APP 🐛 DEBUG: Debug message");
    }

    [Fact]
    public void LogLevelHierarchy_ShouldWorkCorrectly()
    {
        var errorLogger = new ConsoleLogger("ERR", LogLevel.Error);
        var warningLogger = new ConsoleLogger("WARN", LogLevel.Warning);
        var infoLogger = new ConsoleLogger("INFO", LogLevel.Information);
        var debugLogger = new ConsoleLogger("DEBUG", LogLevel.Debug);

        // Error logger should only output errors
        CaptureConsoleOutput(() => errorLogger.LogError("error")).Should().NotBeEmpty();
        CaptureConsoleOutput(() => errorLogger.LogWarning("warning")).Should().BeEmpty();
        CaptureConsoleOutput(() => errorLogger.LogInformation("info")).Should().BeEmpty();
        CaptureConsoleOutput(() => errorLogger.LogDebug("debug")).Should().BeEmpty();

        // Warning logger should output errors and warnings
        CaptureConsoleOutput(() => warningLogger.LogError("error")).Should().NotBeEmpty();
        CaptureConsoleOutput(() => warningLogger.LogWarning("warning")).Should().NotBeEmpty();
        CaptureConsoleOutput(() => warningLogger.LogInformation("info")).Should().BeEmpty();
        CaptureConsoleOutput(() => warningLogger.LogDebug("debug")).Should().BeEmpty();

        // Info logger should output errors, warnings, and info
        CaptureConsoleOutput(() => infoLogger.LogError("error")).Should().NotBeEmpty();
        CaptureConsoleOutput(() => infoLogger.LogWarning("warning")).Should().NotBeEmpty();
        CaptureConsoleOutput(() => infoLogger.LogInformation("info")).Should().NotBeEmpty();
        CaptureConsoleOutput(() => infoLogger.LogDebug("debug")).Should().BeEmpty();

        // Debug logger should output everything
        CaptureConsoleOutput(() => debugLogger.LogError("error")).Should().NotBeEmpty();
        CaptureConsoleOutput(() => debugLogger.LogWarning("warning")).Should().NotBeEmpty();
        CaptureConsoleOutput(() => debugLogger.LogInformation("info")).Should().NotBeEmpty();
        CaptureConsoleOutput(() => debugLogger.LogDebug("debug")).Should().NotBeEmpty();
    }

    [Fact]
    public void ComplexScenario_ApplicationLogging_ShouldWorkCorrectly()
    {
        var logger = new ConsoleLogger("MyApp", LogLevel.Information);

        var errorOutput = CaptureConsoleOutput(() => logger.LogError("Database connection failed"));
        var warningOutput = CaptureConsoleOutput(() => logger.LogWarning("High memory usage detected"));
        var infoOutput = CaptureConsoleOutput(() => logger.LogInformation("User logged in successfully"));
        var debugOutput = CaptureConsoleOutput(() => logger.LogDebug("Variable X = 42")); // Should not output

        errorOutput.Should().Be("MyApp ❌ ERROR: Database connection failed");
        warningOutput.Should().Be("MyApp ⚠️ WARNING: High memory usage detected");
        infoOutput.Should().Be("MyApp User logged in successfully");
        debugOutput.Should().BeEmpty();
    }

    [Fact]
    public void SpecialCharacters_InMessages_ShouldHandleCorrectly()
    {
        var logger = new ConsoleLogger("🚀");
        var message = "Special chars: @#$%^&*(){}[]|\\:;\"'<>,.?/~`";

        var output = CaptureConsoleOutput(() => logger.LogInformation(message));

        output.Should().Be($"🚀 {message}");
    }

    [Fact]
    public void LongMessages_ShouldHandleCorrectly()
    {
        var logger = new ConsoleLogger("TEST");
        var longMessage = new string('A', 1000);

        var output = CaptureConsoleOutput(() => logger.LogInformation(longMessage));

        output.Should().Be($"TEST {longMessage}");
    }

    [Theory]
    [InlineData(LogLevel.Error, "Only errors")]
    [InlineData(LogLevel.Warning, "Errors and warnings")]
    [InlineData(LogLevel.Information, "Errors, warnings and info")]
    [InlineData(LogLevel.Debug, "Everything")]
    public void LogLevel_DescriptiveTest_ShouldBehavePredictably(LogLevel logLevel, string description)
    {
        var logger = new ConsoleLogger("TEST", logLevel);

        // Test that log level filtering works as expected - description: {0}
        description.Should().NotBeNullOrEmpty(); // Use the description parameter
        var errorOutput = CaptureConsoleOutput(() => logger.LogError("Error"));
        var warningOutput = CaptureConsoleOutput(() => logger.LogWarning("Warning"));
        var infoOutput = CaptureConsoleOutput(() => logger.LogInformation("Info"));
        var debugOutput = CaptureConsoleOutput(() => logger.LogDebug("Debug"));

        // Errors should always be logged
        errorOutput.Should().NotBeEmpty();

        // Warnings should be logged for Warning level and above
        if (logLevel >= LogLevel.Warning)
            warningOutput.Should().NotBeEmpty();
        else
            warningOutput.Should().BeEmpty();

        // Info should be logged for Information level and above
        if (logLevel >= LogLevel.Information)
            infoOutput.Should().NotBeEmpty();
        else
            infoOutput.Should().BeEmpty();

        // Debug should only be logged for Debug level
        if (logLevel >= LogLevel.Debug)
            debugOutput.Should().NotBeEmpty();
        else
            debugOutput.Should().BeEmpty();
    }
}

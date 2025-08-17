using System.Text;
using Discord;
using Microsoft.Extensions.Logging;

namespace DiscordLogger;

internal class DiscordLogger(string name, DiscordLoggerOptions options, IDiscordLoggingService loggingService) : ILogger
{
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= options.MinLogLevel;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var message = formatter(state, exception);

        // Character limit for embed descriptions
        var needsFile = message.Length > 4096;

        var embed = new EmbedBuilder()
        {
            Title = $"[{GetLogLevelName(logLevel)}] {name}",
            Description = needsFile ? "*Log message is too big and is attached as file*" : message,
            Color = logLevel switch
            {
                LogLevel.Information => Color.LightGrey,
                LogLevel.Warning => Color.Gold,
                LogLevel.Error => Color.Red,
                LogLevel.Critical => Color.DarkRed,
                _ => Color.DarkGrey
            },
            Timestamp = DateTime.Now,
        };

        var logMessage = new DiscordLogMessage
        {
            Embed = embed.Build(),
        };

        if (needsFile)
        {
            logMessage.File = new MemoryStream(Encoding.UTF8.GetBytes(message));
        }

        loggingService.AddLogMessage(logMessage);
    }

    private static string GetLogLevelName(LogLevel logLevel) => logLevel switch
    {
        LogLevel.Information => "INFO",
        LogLevel.Warning => "WARN",
        LogLevel.Error => "ERROR",
        LogLevel.Critical => "CRITICAL",
        _ => "UNKNOWN"
    };
}
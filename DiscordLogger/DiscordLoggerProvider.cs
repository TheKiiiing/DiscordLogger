using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscordLogger;

[ProviderAlias("Discord")]
internal sealed class DiscordLoggerProvider(IOptions<DiscordLoggerOptions> options, IDiscordLoggingService loggingService) : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, DiscordLogger> loggers = new();

    public ILogger CreateLogger(string categoryName)
    {
        return loggers.GetOrAdd(
            categoryName,
            name => new DiscordLogger(name, options.Value, loggingService)
        );
    }

    public void Dispose()
    {
        loggers.Clear();
    }
}
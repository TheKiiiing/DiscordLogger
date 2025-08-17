using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace DiscordLogger;

/// <summary>
/// Represents the configuration options for the Discord Logger.
/// </summary>
public class DiscordLoggerOptions
{
    /// Defines the Discord Webhook URL used to send log messages.
    /// This URL is essential for the logger to communicate with the Discord channel.
    /// It must be a valid, active Discord webhook endpoint.
    /// The property must be explicitly set; it cannot be null or empty.
    [Required]
    public string WebhookUrl { get; set; } = null!;

    /// Specifies the minimum logging level for messages to be written to the Discord logging service.
    /// Only messages with a log level equal to or higher than the defined `MinLogLevel` will be logged.
    /// This property allows filtering of log messages based on severity.
    /// The default value is `LogLevel.Information`.
    public LogLevel MinLogLevel { get; set; } = LogLevel.Information;

    /// Specifies the log level at which mentions are included in the log messages sent to Discord.
    /// This property helps in highlighting critical log messages that require immediate attention.
    public LogLevel MentionLogLevel { get; set; } = LogLevel.Error;

    /// Specifies the interval at which messages will be sent to Discord.
    /// The default value is 2 seconds.
    public TimeSpan SendInterval  { get; set; } = TimeSpan.FromSeconds(2);

    /// Defines the maximum number of log messages that can be processed and sent in each interval.
    /// Once the message limit is reached within the interval, additional messages will be discarded
    /// until the next interval begins. This helps prevent overloading the logging service or exceeding
    /// limits on the Discord webhook. The default value is 2000.
    public int IntervalMessageLimit { get; set; } = 2_000;
}
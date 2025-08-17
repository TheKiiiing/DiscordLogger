using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DiscordLogger;

public static class LoggingBuilderExtensions
{
    /// <summary>
    /// Adds a Discord logger to the logging builder, utilizing the provided configuration section.
    /// </summary>
    /// <param name="builder">The logging builder to which the Discord logger will be added.</param>
    /// <param name="configurationSection">The configuration section containing settings for the Discord logger. See <see cref="DiscordLoggerOptions"/> for all available config options</param>
    /// <returns>The updated logging builder with the Discord logger added.</returns>
    public static ILoggingBuilder AddDiscord(this ILoggingBuilder builder, IConfigurationSection configurationSection)
    {
        builder.AddDiscord();
        builder.Services.AddOptions<DiscordLoggerOptions>()
            .Bind(configurationSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return builder;
    }

    /// <summary>
    /// Adds a Discord logger to the logging builder using a configuration action for setup.
    /// </summary>
    /// <param name="builder">The logging builder to which the Discord logger will be added.</param>
    /// <param name="configure">An action to configure the settings for the Discord logger using the <see cref="DiscordLoggerOptions"/> class.</param>
    /// <returns>The updated logging builder with the Discord logger added.</returns>
    public static ILoggingBuilder AddDiscord(this ILoggingBuilder builder, Action<DiscordLoggerOptions> configure)
    {
        builder.AddDiscord();
        builder.Services.Configure<DiscordLoggerOptions>(configure);

        return builder;
    }

    /// <summary>
    /// Adds a Discord logger to the logging builder.
    /// </summary>
    /// <param name="builder">The logging builder to which the Discord logger will be added.</param>
    /// <returns>The updated logging builder with the Discord logger added.</returns>
    public static ILoggingBuilder AddDiscord(this ILoggingBuilder builder)
    {
        builder.Services.AddHostedService<DiscordLoggingService>();
        builder.Services.AddSingleton<ILoggerProvider, DiscordLoggerProvider>();

        return builder;
    }
}
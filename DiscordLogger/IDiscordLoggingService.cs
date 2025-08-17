namespace DiscordLogger;

internal interface IDiscordLoggingService
{
    void AddLogMessage(DiscordLogMessage message);
}
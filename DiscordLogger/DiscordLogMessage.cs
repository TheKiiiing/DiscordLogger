using Discord;

namespace DiscordLogger;

internal class DiscordLogMessage
{
    public required Embed Embed { get; init; }
    public Stream? File { get; set; }
    public bool Mention { get; set; }
}
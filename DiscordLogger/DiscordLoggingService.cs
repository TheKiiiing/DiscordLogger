using System.Collections.Concurrent;
using Discord;
using Discord.Webhook;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DiscordLogger;

internal class DiscordLoggingService(IOptions<DiscordLoggerOptions> options) : IDiscordLoggingService, IHostedService
{
    private const int MAX_EMBEDS = 10;
    private const string MENTION = "@everyone";

    private readonly CancellationTokenSource _cts = new();
    private readonly ConcurrentQueue<DiscordLogMessage> _logMessages = new();

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _ = Task.Factory.StartNew(SendLoop, TaskCreationOptions.LongRunning);

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _cts.CancelAsync();
    }

    private async Task SendLoop()
    {
        var client = new DiscordWebhookClient(options.Value.WebhookUrl);
        var embeds = new List<Embed>(MAX_EMBEDS);
        var currentBatch = new List<DiscordLogMessage>(options.Value.IntervalMessageLimit);

        while (!_cts.IsCancellationRequested)
        {
            await Task.Delay(options.Value.SendInterval, _cts.Token);

            try
            {
                currentBatch.Clear();
                while (_logMessages.TryDequeue(out var message))
                {
                    currentBatch.Add(message);
                }

                await DoSend(currentBatch, client, embeds);
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync($"[DISCORD LOGGER] Error sending log message to Discord: {e}");
            }
        }
    }

    private async Task DoSend(List<DiscordLogMessage> currentBatch, DiscordWebhookClient client, List<Embed> embeds)
    {
        var mention = options.Value.MentionOverride ?? MENTION;

        foreach (var message in currentBatch)
        {
            var text = $"{mention} {message.Embed.Title}";
            if (message.File != null)
            {
                await client.SendFileAsync(
                    message.File,
                    filename: $"log_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt",
                    text: text,
                    embeds: [message.Embed],
                    allowedMentions: AllowedMentions.All
                );
            }
            else if (message.Mention)
            {
                await client.SendMessageAsync(text, embeds: [message.Embed], allowedMentions: AllowedMentions.All);
            }
            else
            {
                embeds.Add(message.Embed);
            }

            if (embeds.Count >= MAX_EMBEDS)
            {
                await SendEmbeds();
            }
        }

        // Send remaining embeds
        await SendEmbeds();
        return;

        async Task SendEmbeds()
        {
            if (embeds.Count > 0)
            {
                await client.SendMessageAsync(embeds: embeds);
                embeds.Clear();
            }
        }
    }

    public void AddLogMessage(DiscordLogMessage message)
    {
        if (_logMessages.Count >= options.Value.IntervalMessageLimit)
        {
            // Drop the log message
            return;
        }

        _logMessages.Enqueue(message);
    }
}
using System.Collections.Concurrent;
using Discord;
using Discord.Webhook;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DiscordLogger;

internal class DiscordLoggingService(IOptions<DiscordLoggerOptions> options) : IDiscordLoggingService, IHostedService
{
    private readonly CancellationTokenSource _cts = new();
    private readonly ConcurrentQueue<DiscordLogMessage> _logMessages = new();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _ = Task.Factory.StartNew(SendLoop, TaskCreationOptions.LongRunning);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _cts.CancelAsync();
    }

    private async Task SendLoop()
    {
        const int max_embeds = 10;
        var client = new DiscordWebhookClient(options.Value.WebhookUrl);
        var embeds = new List<Embed>(max_embeds);
        var currentBatch = new List<DiscordLogMessage>(options.Value.IntervalMessageLimit);

        while (!_cts.IsCancellationRequested)
        {
            await Task.Delay(options.Value.SendInterval, _cts.Token);

            currentBatch.Clear();
            while (_logMessages.TryDequeue(out var message))
            {
                currentBatch.Add(message);
            }
            
            
            foreach (var message in currentBatch)
            {
                if (message.File != null)
                {
                    await client.SendFileAsync(message.File, $"log_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt", "Log file", embeds: [message.Embed]);
                }
                else
                {
                    embeds.Add(message.Embed);
                }
                
                if (embeds.Count >= max_embeds)
                {
                    await SendEmbeds();
                }
            }
            
            // Send remaining embeds
            await SendEmbeds();
        }
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
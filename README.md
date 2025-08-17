# Discord Logger

A .NET 8.0 library that provides Discord webhook logging capabilities for .NET applications using the Microsoft.Extensions.Logging framework.

Inspired by https://github.com/ithr0n/DiscordLogging


## Setup

Add Discord logging to your application's service collection:

```csharp
// Program.cs or Startup.cs

using DiscordLogging;

// Configure your Discord webhook URL and other options directly
builder.Logging.AddDiscord(options =>
{
    options.WebhookUrl = "https://discord.com/api/webhooks/your-webhook";
});

// OR bind to a configuration section
builder.Logging.AddDiscord(builder.Configuration.GetSection("Logging:Discord"));
```


## Configuration

All available config options can be found in [DiscordLoggerOptions.cs](DiscordLogging/DiscordLoggerOptions.cs).
Configure the Discord logger options in your `appsettings.json`:

```json
{
  "Logging": {
    "Discord": {
      "WebhookUrl": "https://discord.com/api/webhooks/your-webhook",
      "MinLogLevel": "Information"
      // Add other configuration options as needed
    }
  }
}
```


## License

This project is licensed under the [MIT License](LICENSE).



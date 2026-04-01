using Discord;
using Discord.Net.Template.Utils;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Discord.Net.Template.Services;

public class BotService(
    DiscordSocketClient client,
    IConfiguration config,
    ILogger<BotService> logger,
    IEnumerable<IModuleHandler> handlers
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        client.Log += HandleLogAsync;

        foreach (var handler in handlers)
        {
            await handler.InitializeAsync();
        }

        await client.LoginAsync(TokenType.Bot, config["Discord:Token"]);
        await client.StartAsync();

        await Task.Delay(-1, stoppingToken);
    }

    private Task HandleLogAsync(LogMessage message)
    {
        logger.Log(message);

        return Task.CompletedTask;
    }
}

using Discord;
using Discord.Net.Template.Extensions;
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

        var token = config["Discord:Token"];

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException(
                "Discord:Token is not configured. Set it with `dotnet user-secrets set Discord:Token <token>` "
                    + "or the Discord__Token environment variable."
            );
        }

        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

        await Task.Delay(-1, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await base.StopAsync(cancellationToken);

        await client.StopAsync();
        await client.LogoutAsync();
    }

    private Task HandleLogAsync(LogMessage message)
    {
        logger.Log(message);

        return Task.CompletedTask;
    }
}

using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

IConfiguration config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var discordConfig = new DiscordSocketConfig()
{
    GatewayIntents =
        GatewayIntents.All
        & ~GatewayIntents.GuildPresences
        & ~GatewayIntents.GuildScheduledEvents
        & ~GatewayIntents.GuildInvites,
    LogLevel = Enum.Parse<LogSeverity>(config["LogLevel"] ?? "Warning"),
};

var client = new DiscordSocketClient(discordConfig);

var host = Host.CreateDefaultBuilder()
    .ConfigureServices(
        (_, services) =>
        {
            services.AddSingleton(client);
            services.BuildServiceProvider();
        }
    )
    .Build();

await client.LoginAsync(TokenType.Bot, config["Bot.Token"]);
await client.StartAsync();

await host.RunAsync();

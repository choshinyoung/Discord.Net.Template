using Discord.Net.Template.Commands;
using Discord.Net.Template.Interactions;
using Discord.Net.Template.Utility;
using Discord.WebSocket;
using Fergun.Interactive;
using Microsoft.Extensions.DependencyInjection;
using EventHandler = Discord.Net.Template.Events.EventHandler;

namespace Discord.Net.Template;

public static class Bot
{
    public static readonly bool IsDebugMode = Config.GetBool("DEBUG_MODE");

    private static readonly DiscordSocketConfig ClientConfig = new()
    {
        GatewayIntents = GatewayIntents.All,
        LogLevel = IsDebugMode ? LogSeverity.Debug : LogSeverity.Info
    };

    public static readonly DiscordSocketClient Client = new(ClientConfig);

    public static readonly IServiceProvider Service = new ServiceCollection()
        .AddSingleton(Client)
        .AddSingleton<InteractiveService>()
        .AddSingleton(InteractionManager.Service)
        .AddSingleton(CommandManager.Service)
        .BuildServiceProvider();

    public static async Task Start()
    {
        EventHandler.Register();

        await InteractionManager.Initialize();
        await CommandManager.Initialize();

        await Client.LoginAsync(TokenType.Bot, Config.Get("TOKEN"));
        await Client.StartAsync();

        await Task.Delay(-1);
    }
}
using Discord.Net.Template.Interactions;
using Discord.Net.Template.Utility;
using Discord.WebSocket;
using EventHandler = Discord.Net.Template.Events.EventHandler;

namespace Discord.Net.Template;

public static class Bot
{
    private static readonly DiscordSocketConfig ClientConfig = new()
    {
        GatewayIntents = GatewayIntents.All,
        LogLevel = Config.GetBool("DEBUG_MODE") ? LogSeverity.Debug : LogSeverity.Info
    };

    public static readonly DiscordSocketClient Client = new(ClientConfig);

    public static async Task Start()
    {
        EventHandler.Register();

        await InteractionManager.Initialize();

        await Client.LoginAsync(TokenType.Bot, Config.Get("TOKEN"));
        await Client.StartAsync();

        await Task.Delay(-1);
    }
}
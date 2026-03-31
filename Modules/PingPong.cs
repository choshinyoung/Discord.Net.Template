using Discord.Interactions;
using Discord.Net.Template.Utils;

namespace Discord.Net.Template.Modules;

public class PingPong : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "Just a Ping-Pong Command")]
    public async Task Ping()
    {
        await Context.RespondAsync("Pong!");
    }
}

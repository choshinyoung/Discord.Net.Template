using Discord.Interactions;
using Discord.Net.Template.Extensions;

namespace Discord.Net.Template.Interactions;

public class PingPong : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "Just a Ping-Pong Command")]
    public async Task Ping()
    {
        await Context.RespondAsync("Pong!");
    }
}
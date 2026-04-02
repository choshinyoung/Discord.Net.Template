using Discord.Interactions;
using Discord.Net.Template.Extensions;
using Microsoft.Extensions.Logging;

namespace Discord.Net.Template.Modules.Interactions;

public class PingPong(ILogger<PingPong> logger) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "Just a Ping-Pong Command")]
    public async Task Ping()
    {
        logger.LogInformation("Executing Ping pong command!");

        await Context.RespondAsync("Pong!");
    }
}

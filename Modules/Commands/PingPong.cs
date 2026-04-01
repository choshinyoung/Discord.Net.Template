using Discord.Commands;
using Discord.Net.Template.Utils;
using Microsoft.Extensions.Logging;

namespace Discord.Net.Template.Modules.Commands;

public class PingPong(ILogger<PingPong> logger) : ModuleBase<SocketCommandContext>
{
    [Command("ping")]
    [Summary("Just a Ping-Pong Command")]
    public async Task Ping()
    {
        logger.LogInformation("Executing Ping pong command!");

        await Context.ReplyAsync("Pong!");
    }
}

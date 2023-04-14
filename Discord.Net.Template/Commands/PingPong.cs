using Discord.Commands;
using Discord.Net.Template.Extensions;

namespace Discord.Net.Template.Commands;

public class PingPong : ModuleBase<SocketCommandContext>
{
    [Command("ping")]
    public async Task Ping()
    {
        await Context.ReplyAsync("Pong!");
    }
}
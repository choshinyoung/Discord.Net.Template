using Discord.Commands;
using Discord.Net.Template.Attributes;
using Discord.Net.Template.Extensions;

namespace Discord.Net.Template.Commands;

[Order(0)]
public class PingPong : ModuleBase<SocketCommandContext>
{
    [Command("ping")]
    [Summary("Just a Ping-Pong Command")]
    public async Task Ping()
    {
        await Context.ReplyAsync("Pong!");
    }
}
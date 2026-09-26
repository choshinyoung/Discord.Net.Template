using Discord;
using Discord.Commands;
using Discord.Net.Template.Attributes;
using Discord.Net.Template.Extensions;
using Discord.Net.Template.Services;
using Microsoft.Extensions.Configuration;

namespace Discord.Net.Template.Modules.Commands;

[Group("help")]
[Order(1)]
public class Help(IConfiguration config, CommandService command, PaginatorHandler paginator)
    : ModuleBase<SocketCommandContext>
{
    [Command("")]
    [Summary("List of commands")]
    public async Task HelpCommand()
    {
        await paginator.InitPaginator(Context, "help.command");
    }

    [Command("")]
    [Summary("Checks description for specific commands")]
    public async Task HelpSingleCommand([Remainder, Name("command")] string commandName)
    {
        var commands = command.FindHelpCommands(commandName);

        if (commands.Count == 0)
        {
            await Context.ReplyAsync("Cannot find matching commands");

            return;
        }

        var embed = new EmbedBuilder().WithDefaultColor().WithTitle($"`{commandName}` commands");

        foreach (var command in commands)
        {
            embed.AddField(command.GetUsage(config["Discord:Prefix"]), command.Summary);
        }

        await Context.ReplyEmbedAsync(embed.Build());
    }
}

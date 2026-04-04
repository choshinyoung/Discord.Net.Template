using Discord;
using Discord.Commands;
using Discord.Net.Template.Attributes;
using Discord.Net.Template.Extensions;
using Discord.Net.Template.Utils;

namespace Discord.Net.Template.Modules.Commands;

[Group("help")]
[Order(1)]
public class Help(CommandService command) : ModuleBase<SocketCommandContext>
{
    [Command("")]
    [Summary("List of commands")]
    public async Task HelpCommand()
    {
        var modules = command.GetModules();

        var embed = BuildPageEmbed(modules, 0);
        var component = BuildPageButtons(Context.User.Id, 0, modules.Count);

        await Context.ReplyEmbedAsync(embed.Build(), component: component);
    }

    [Command("")]
    [Summary("Checks description for specific commands")]
    public async Task HelpSingleCommand([Remainder, Name("command")] string commandName)
    {
        var modules = command.GetModules();

        var commands = modules
            .SelectMany(x => x.GetCommands())
            .Where(c =>
                !InfoUtil.HaveAttribute<HideInHelpAttribute>(c) && !string.IsNullOrEmpty(c.Summary)
            )
            .Where(c => c.Name == commandName || c.GetFullName() == commandName)
            .ToList();

        if (commands.Count == 0)
        {
            await Context.ReplyAsync("Cannot find matching commands");

            return;
        }

        var embed = new EmbedBuilder().WithDefaultColor().WithTitle($"`{commandName}` commands");

        foreach (var command in commands)
        {
            var parameters = string.Join(
                ' ',
                command.Parameters.Where(p => p.Name != "").Select(p => $"`{p.Name}`")
            );

            embed.AddField($"/{command.GetFullName()} {parameters}", command.Summary);
        }

        await Context.ReplyEmbedAsync(embed.Build());
    }
}

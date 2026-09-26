using Discord;
using Discord.Commands;
using Discord.Net.Template.Attributes;
using Discord.Net.Template.Extensions;
using Microsoft.Extensions.Configuration;

namespace Discord.Net.Template.Modules.Paginators;

public class CommandHelp(IConfiguration config, CommandService command) : Paginator
{
    [Paginator("help.command")]
    public (Embed, bool) HelpCommand()
    {
        var modules = command.GetModules();
        var module = modules[Math.Clamp(Index, 0, modules.Count - 1)];

        var embed = new EmbedBuilder().WithDefaultColor().WithTitle(module.Name);

        var commands = module.Commands.Where(c => c.IsVisibleInHelp()).DistinctBy(c => c.Name);

        foreach (var command in commands)
        {
            embed.AddField(
                command.GetUsage(config["Discord:Prefix"]),
                command.Summary.Split('\n')[0]
            );
        }

        embed.WithFooter($"Page {Index + 1}/{modules.Count}");

        return (embed.Build(), Index >= modules.Count - 1);
    }
}

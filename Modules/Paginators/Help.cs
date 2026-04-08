using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.Net.Template.Attributes;
using Discord.Net.Template.Extensions;
using Discord.Net.Template.Utils;
using Microsoft.Extensions.Configuration;

namespace Discord.Net.Template.Modules.Paginators;

public class Help(IConfiguration config, InteractionService interaction, CommandService command)
    : Paginator
{
    [Paginator("help.interaction")]
    public (Embed, bool) HelpInteraction()
    {
        var modules = interaction.GetModules();
        var module = modules[Math.Clamp(Index, 0, modules.Count - 1)];

        var embed = new EmbedBuilder()
            .WithDefaultColor()
            .WithTitle(module.SlashGroupName ?? module.Name);

        var commands = module
            .SlashCommands.Where(c =>
                !InfoUtil.HaveAttribute<HideInHelpAttribute>(c)
                && !string.IsNullOrEmpty(c.Description)
            )
            .DistinctBy(c => c.Name)
            .ToList();

        foreach (var command in commands)
        {
            var parameters = string.Join(
                ' ',
                command
                    .Parameters.Where(p => p.Name != "")
                    .Select(p => p.IsRequired ? $"`{p.Name}`" : $"`[{p.Name}]`")
            );

            embed.AddField(
                $"/{command.GetFullName()} {parameters}",
                command.Description.Split('\n')[0]
            );
        }

        embed.WithFooter($"Page {Index + 1}/{modules.Count}");

        return (embed.Build(), Index >= modules.Count - 1);
    }

    [Paginator("help.command")]
    public (Embed, bool) HelpCommand()
    {
        var modules = command.GetModules();
        var module = modules[Math.Clamp(Index, 0, modules.Count - 1)];

        var embed = new EmbedBuilder().WithDefaultColor().WithTitle(module.Name);

        var commands = module
            .Commands.Where(c =>
                !InfoUtil.HaveAttribute<HideInHelpAttribute>(c) && !string.IsNullOrEmpty(c.Summary)
            )
            .DistinctBy(c => c.Name)
            .ToList();

        foreach (var command in commands)
        {
            var parameters = string.Join(
                ' ',
                command.Parameters.Where(p => p.Name != "").Select(p => $"`{p.Name}`")
            );

            embed.AddField(
                $"{config["Discord:Prefix"]}{command.GetFullName()} {parameters}",
                command.Summary.Split('\n')[0]
            );
        }

        embed.WithFooter($"Page {Index + 1}/{modules.Count}");

        return (embed.Build(), Index >= modules.Count - 1);
    }
}

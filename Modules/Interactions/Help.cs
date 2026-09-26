using Discord;
using Discord.Interactions;
using Discord.Net.Template.Attributes;
using Discord.Net.Template.Extensions;
using Discord.Net.Template.Modules.Interactions.AutoCompletes;
using Discord.Net.Template.Services;

namespace Discord.Net.Template.Modules.Interactions;

[Order(1)]
public class Help(InteractionService interaction, PaginatorHandler paginator)
    : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("help", "List of slash commands")]
    public async Task HelpCommand([Autocomplete(typeof(HelpAutoComplete))] string? command = null)
    {
        if (!string.IsNullOrEmpty(command))
        {
            await HelpSingleCommand(command);

            return;
        }

        await paginator.InitPaginator(Context, "help.interaction");
    }

    public async Task HelpSingleCommand(string commandName)
    {
        var commands = interaction.FindHelpCommands(commandName);

        if (commands.Count == 0)
        {
            await Context.RespondAsync("Cannot find matching commands", true);

            return;
        }

        var embed = new EmbedBuilder().WithDefaultColor().WithTitle($"`{commandName}` commands");

        foreach (var command in commands)
        {
            embed.AddField(command.GetUsage(), command.Description);
        }

        await Context.RespondEmbedAsync(embed.Build());
    }
}

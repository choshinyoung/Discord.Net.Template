using Discord.Interactions;
using Discord.Net.Template.Attributes;
using Discord.Net.Template.Extensions;
using Discord.Net.Template.Modules.Interactions.AutoCompletes;
using Discord.Net.Template.Services;
using Discord.Net.Template.Utils;

namespace Discord.Net.Template.Modules.Interactions;

[Order(1)]
public class Help(InteractionHandler interactionHandler)
    : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("help", "List of slash commands")]
    public async Task HelpCommand(
        [Autocomplete(typeof(HelpAutoComplete))] string? commandName = null
    ) { }

    public async Task Command(string commandName)
    {
        var modules = interactionHandler.GetModules();

        var commands = modules
            .SelectMany(x => x.GetCommands())
            .Where(c =>
                !InfoUtil.HaveAttribute<HideInHelpAttribute>(c)
                && !string.IsNullOrEmpty(c.Description)
            )
            .Where(c => c.Name == commandName || c.GetFullName() == commandName)
            .ToList();

        if (commands.Count == 0)
        {
            await Context.RespondAsync("Cannot find matching commands", true);

            return;
        }

        var embed = new EmbedBuilder().WithDefaultColor().WithTitle($"`{commandName}` commands");

        foreach (var command in commands)
        {
            var parameters = string.Join(
                ' ',
                command.Parameters.Where(p => p.Name != "").Select(p => $"`{p.Name}`")
            );

            embed.AddField($"/{command.GetFullName()} {parameters}", command.Description);
        }

        await Context.RespondEmbedAsync(embed.Build());
    }
}

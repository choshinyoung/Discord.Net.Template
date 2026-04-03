using Discord;
using Discord.Interactions;
using Discord.Net.Template.Attributes;
using Discord.Net.Template.Extensions;
using Discord.Net.Template.Modules.Interactions.AutoCompletes;
using Discord.Net.Template.Utils;

namespace Discord.Net.Template.Modules.Interactions;

[Order(1)]
public class Help(InteractionService interaction) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("help", "List of slash commands")]
    public async Task HelpCommand([Autocomplete(typeof(HelpAutoComplete))] string? command = null)
    {
        if (!string.IsNullOrEmpty(command))
        {
            await HelpSingleCommand(command);

            return;
        }

        var modules = interaction.GetModules();

        if (modules.Count == 0)
        {
            await Context.RespondAsync("No modules found", true);

            return;
        }

        var embed = BuildPageEmbed(modules, 0);
        var component = BuildPageButtons(Context.User.Id, 0, modules.Count);

        await Context.RespondEmbedAsync(embed.Build(), component: component);
    }

    [ComponentInteraction("help:page:*,*")]
    public async Task HelpPageButtonClick(string userId, string pageIndex)
    {
        if (
            !ulong.TryParse(userId, out var ownerId)
            || !int.TryParse(pageIndex, out var targetIndex)
        )
        {
            return;
        }

        if (Context.User.Id != ownerId)
        {
            return;
        }

        var modules = interaction.GetModules();

        if (modules.Count == 0)
        {
            await RespondAsync("No modules found", ephemeral: true);

            return;
        }

        var index = Math.Clamp(targetIndex, 0, modules.Count - 1);
        var embed = BuildPageEmbed(modules, index);
        var component = BuildPageButtons(ownerId, index, modules.Count);

        await ModifyOriginalResponseAsync(message =>
        {
            message.Embed = embed.Build();
            message.Components = component;
        });
    }

    private static EmbedBuilder BuildPageEmbed(List<ModuleInfo> modules, int index)
    {
        var module = modules[index];
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

        if (commands.Count == 0)
        {
            embed.WithDescription("No visible slash commands in this module.");
        }

        embed.WithFooter($"Page {index + 1}/{modules.Count}");

        return embed;
    }

    private static MessageComponent BuildPageButtons(ulong userId, int currentIndex, int totalPages)
    {
        var firstIndex = 0;
        var previousIndex = Math.Max(currentIndex - 1, 0);
        var nextIndex = Math.Min(currentIndex + 1, totalPages - 1);
        var lastIndex = Math.Max(totalPages - 1, 0);

        return new ComponentBuilder()
            .WithButton(
                "⏪",
                $"help:page:{userId},{firstIndex}",
                ButtonStyle.Secondary,
                disabled: currentIndex == firstIndex
            )
            .WithButton(
                "◀",
                $"help:page:{userId},{previousIndex}",
                ButtonStyle.Primary,
                disabled: currentIndex == firstIndex
            )
            .WithButton(
                $"{currentIndex + 1}/{totalPages}",
                "help:page:noop,0",
                ButtonStyle.Secondary,
                disabled: true
            )
            .WithButton(
                "▶",
                $"help:page:{userId},{nextIndex}",
                ButtonStyle.Primary,
                disabled: currentIndex == lastIndex
            )
            .WithButton(
                "⏩",
                $"help:page:{userId},{lastIndex}",
                ButtonStyle.Secondary,
                disabled: currentIndex == lastIndex
            )
            .Build();
    }

    public async Task HelpSingleCommand(string commandName)
    {
        var modules = interaction.GetModules();

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

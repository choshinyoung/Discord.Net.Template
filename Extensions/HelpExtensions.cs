using Discord.Commands;
using Discord.Interactions;
using Discord.Net.Template.Attributes;

namespace Discord.Net.Template.Extensions;

public static class HelpExtensions
{
    public static bool IsVisibleInHelp(this CommandInfo command)
    {
        return !command.Attributes.HasAttribute<HideInHelpAttribute>()
            && !string.IsNullOrEmpty(command.Summary);
    }

    public static bool IsVisibleInHelp(this SlashCommandInfo command)
    {
        return !command.Attributes.HasAttribute<HideInHelpAttribute>()
            && !string.IsNullOrEmpty(command.Description);
    }

    public static string GetUsage(this CommandInfo command, string? prefix)
    {
        var parameters = command
            .Parameters.Where(p => p.Name != "")
            .Select(p => p.IsOptional ? $"`[{p.Name}]`" : $"`{p.Name}`");

        return $"{prefix}{command.GetFullName()} {string.Join(' ', parameters)}".TrimEnd();
    }

    public static string GetUsage(this SlashCommandInfo command)
    {
        var parameters = command
            .Parameters.Where(p => p.Name != "")
            .Select(p => p.IsRequired ? $"`{p.Name}`" : $"`[{p.Name}]`");

        return $"/{command.GetFullName()} {string.Join(' ', parameters)}".TrimEnd();
    }

    public static List<CommandInfo> FindHelpCommands(this CommandService command, string name)
    {
        return
        [
            .. command
                .GetModules()
                .SelectMany(m => m.GetCommands())
                .Where(c => c.IsVisibleInHelp() && (c.Name == name || c.GetFullName() == name)),
        ];
    }

    public static List<SlashCommandInfo> FindHelpCommands(
        this InteractionService interaction,
        string name
    )
    {
        return
        [
            .. interaction
                .GetModules()
                .SelectMany(m => m.GetCommands())
                .Where(c => c.IsVisibleInHelp() && (c.Name == name || c.GetFullName() == name)),
        ];
    }
}

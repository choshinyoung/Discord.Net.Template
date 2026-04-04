using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.Net.Template.Attributes;
using Discord.Net.Template.Utils;

namespace Discord.Net.Template.Extensions;

public static class ModuleExtensions
{
    public static List<Interactions.ModuleInfo> GetModules(this InteractionService interaction)
    {
        List<Interactions.ModuleInfo> modules =
        [
            .. interaction.Modules.Where(m =>
                !m.IsSubModule && !InfoUtil.HaveAttribute<HideInHelpAttribute>(m)
            ),
        ];
        modules.Sort((m1, m2) => m1.GetOrder().CompareTo(m2.GetOrder()));

        return modules;
    }

    public static List<SlashCommandInfo> GetCommands(this Interactions.ModuleInfo module)
    {
        return
        [
            .. module.SlashCommands,
            .. module.SubModules.OrderBy(GetOrder).SelectMany(GetCommands),
        ];
    }

    public static int GetOrder(this Interactions.ModuleInfo module)
    {
        return InfoUtil.HaveAttribute<OrderAttribute>(module)
            ? InfoUtil.GetAttribute<OrderAttribute>(module).Order
            : int.MaxValue;
    }

    public static string GetFullName(this ICommandInfo command)
    {
        return $"{command.Module.GetParentName()} {command.Name}".Trim();
    }

    private static string GetParentName(this Interactions.ModuleInfo? module)
    {
        if (module is null)
        {
            return "";
        }

        return $"{module.Parent.GetParentName()} {module.SlashGroupName}".Trim();
    }

    public static List<Commands.ModuleInfo> GetModules(this CommandService command)
    {
        List<Commands.ModuleInfo> modules =
        [
            .. command.Modules.Where(m =>
                !m.IsSubmodule && !InfoUtil.HaveAttribute<HideInHelpAttribute>(m)
            ),
        ];
        modules.Sort((m1, m2) => GetOrder(m1).CompareTo(GetOrder(m2)));

        return modules;
    }

    public static List<CommandInfo> GetCommands(this Commands.ModuleInfo module)
    {
        return [.. module.Commands, .. module.Submodules.OrderBy(GetOrder).SelectMany(GetCommands)];
    }

    public static int GetOrder(this Commands.ModuleInfo module)
    {
        return InfoUtil.HaveAttribute<OrderAttribute>(module)
            ? InfoUtil.GetAttribute<OrderAttribute>(module).Order
            : int.MaxValue;
    }

    public static string GetFullName(this CommandInfo command)
    {
        return $"{command.Module.GetParentName()} {command.Name}".Trim();
    }

    private static string GetParentName(this Commands.ModuleInfo? module)
    {
        return module is null ? "" : $"{module.Parent.GetParentName()} {module.Group}".Trim();
    }
}

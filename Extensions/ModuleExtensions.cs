using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.Net.Template.Attributes;
using Discord.Net.Template.Utils;

namespace Discord.Net.Template.Extensions;

public static class ModuleExtensions
{
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
}

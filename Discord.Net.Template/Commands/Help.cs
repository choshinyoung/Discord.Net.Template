using Discord.Commands;
using Discord.Net.Template.Attributes;
using Discord.Net.Template.Extensions;
using Discord.Net.Template.Utility;
using Fergun.Interactive;
using Fergun.Interactive.Pagination;

namespace Discord.Net.Template.Commands;

[Order(1)]
public class Help : ModuleBase<SocketCommandContext>
{
    public InteractiveService? Interactive { get; set; }

    [Command("help")]
    [Summary("List of commands")]
    public async Task CommandList()
    {
        var modules = GetCommandModules();

        var paginator = new LazyPaginatorBuilder()
            .AddUser(Context.User)
            .AddOption(new Emoji("◀"), PaginatorAction.Backward)
            .AddOption(new Emoji("⏪"), PaginatorAction.SkipToStart, ButtonStyle.Secondary)
            .AddOption(new Emoji("▶"), PaginatorAction.Forward)
            .WithPageFactory(GeneratePage)
            .WithMaxPageIndex(modules.Count - 1)
            .WithActionOnTimeout(ActionOnStop.DisableInput)
            .WithCacheLoadedPages(false)
            .Build();

        await Interactive!.SendPaginatorAsync(paginator, Context.Channel, TimeSpan.FromMinutes(10));

        PageBuilder GeneratePage(int index)
        {
            var page = new PageBuilder()
                .WithDefaultColor()
                .WithTitle(modules[index].Name);

            var commands = modules[index].Commands
                .Where(c => !InfoUtility.HaveAttribute<HideInHelpAttribute>(c) && !string.IsNullOrEmpty(c.Summary))
                .DistinctBy(c => c.Name)
                .ToList();

            foreach (var command in commands)
            {
                var parameters = string.Join(' ',
                    command.Parameters
                        .Where(p => p.Name != "")
                        .Select(p => $"`{p.Name}`"));

                page.AddField(
                    $"{CommandManager.Prefix}{GetFullCommandName(command)} {parameters}",
                    command.Summary.Split('\n')[0]);
            }

            return page;
        }
    }

    [Command("help")]
    [Summary("Checks description for specific commands")]
    public async Task Command([Remainder] [Name("command")] string commandName)
    {
        var modules = GetCommandModules();

        var commands = modules
            .SelectMany(GetCommandsFromModule)
            .Where(c => !InfoUtility.HaveAttribute<HideInHelpAttribute>(c) && !string.IsNullOrEmpty(c.Summary))
            .Where(c => c.Name == commandName || GetFullCommandName(c) == commandName)
            .ToList();

        if (!commands.Any())
        {
            await Context.ReplyAsync("Cannot find matching commands");

            return;
        }

        var embed = new EmbedBuilder()
            .WithDefaultColor()
            .WithTitle($"`{commandName}` commands");

        foreach (var command in commands)
        {
            var aliases = string.Join(" ",
                command.Aliases
                    .Where(a => a != command.Name && a != GetFullCommandName(command))
                    .Select(a => $"`{CommandManager.Prefix}{a}`"));

            var parameters = string.Join(' ',
                command.Parameters
                    .Where(p => p.Name != "")
                    .Select(p => $"`{p.Name}`"));

            embed.AddField(
                $"{CommandManager.Prefix}{GetFullCommandName(command)} {parameters}",
                $"{aliases}\n\n{command.Summary}");
        }

        await Context.ReplyEmbedAsync(embed.Build());
    }

    private static List<ModuleInfo> GetCommandModules()
    {
        List<ModuleInfo> modules = CommandManager.Service.Modules
            .Where(m => !m.IsSubmodule && !InfoUtility.HaveAttribute<HideInHelpAttribute>(m))
            .ToList();
        modules.Sort((m1, m2) => GetOrder(m1).CompareTo(GetOrder(m2)));

        return modules;
    }

    private static List<CommandInfo> GetCommandsFromModule(ModuleInfo module)
    {
        return module.Commands.Concat(module.Submodules.OrderBy(GetOrder).SelectMany(GetCommandsFromModule)).ToList();
    }

    private static int GetOrder(ModuleInfo module)
    {
        return InfoUtility.HaveAttribute<OrderAttribute>(module)
            ? InfoUtility.GetAttribute<OrderAttribute>(module).Order
            : int.MaxValue;
    }

    private static string GetFullCommandName(CommandInfo command)
    {
        return $"{GetParentName(command.Module)} {command.Name}".Trim();
    }

    private static string GetParentName(ModuleInfo? module)
    {
        return module is null ? "" : $"{GetParentName(module.Parent)} {module.Group}".Trim();
    }
}
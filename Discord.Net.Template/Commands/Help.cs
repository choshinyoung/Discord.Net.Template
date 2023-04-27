using Discord.Commands;
using Discord.Net.Template.Attributes;
using Discord.Net.Template.Utility;
using Fergun.Interactive;
using Fergun.Interactive.Pagination;

namespace Discord.Net.Template.Commands;

[Order(1)]
public class Help : ModuleBase<SocketCommandContext>
{
    public InteractiveService Interactive { get; set; }

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

        await Interactive.SendPaginatorAsync(paginator, Context.Channel, TimeSpan.FromMinutes(10));

        PageBuilder GeneratePage(int index)
        {
            var page = EmbedUtility.CreatePage(Context, title: modules[index].Name);

            var commands = modules[index].Commands
                .Where(c => !InfoUtility.HaveAttribute<HideInHelpAttribute>(c) && !string.IsNullOrEmpty(c.Summary))
                .DistinctBy(c => c.Name)
                .ToList();

            foreach (var command in commands)
            {
                page.AddField(
                    $"{CommandManager.Prefix}{GetFullCommandName(command)} {string.Join(' ', command.Parameters.Where(p => p.Name != "").Select(p => $"`{p.Name}`"))}",
                    command.Summary.Split('\n')[0]);
            }

            return page;
        }
    }

    private static List<ModuleInfo> GetCommandModules()
    {
        List<ModuleInfo> modules =
            CommandManager.Service.Modules.Where(m => !InfoUtility.HaveAttribute<HideInHelpAttribute>(m)).ToList();
        modules.Sort((m1, m2) => GetOrder(m1).CompareTo(GetOrder(m2)));

        return modules;
    }

    private static int GetOrder(ModuleInfo module)
    {
        return InfoUtility.HaveAttribute<OrderAttribute>(module)
            ? InfoUtility.GetAttribute<OrderAttribute>(module).Order
            : int.MaxValue;
    }

    // private static List<Discord.Interactions.ModuleInfo> GetSlashCommandModules()
    // {
    //     List<Discord.Interactions.ModuleInfo> modules =
    //         InteractionManager.Service.Modules.Where(m => !InfoUtility.HaveAttribute<HideInHelpAttribute>(m)).ToList();
    //     modules.Sort((m1, m2) => GetOrder(m1).CompareTo(GetOrder(m2)));
    //
    //     return modules;
    // }

    // private static int GetOrder(Discord.Interactions.ModuleInfo module)
    // {
    //     return InfoUtility.HaveAttribute<OrderAttribute>(module)
    //         ? InfoUtility.GetAttribute<OrderAttribute>(module).Order
    //         : int.MaxValue;
    // }

    private static string GetFullCommandName(CommandInfo command)
    {
        return $"{GetParentName(command.Module)} {command.Name}".Trim();

        string GetParentName(ModuleInfo? module)
        {
            return module is null ? "" : $"{module.Group} {GetParentName(module.Parent)}".Trim();
        }
    }
}
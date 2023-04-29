using Discord.Interactions;
using Discord.Net.Template.Attributes;
using Discord.Net.Template.Extensions;
using Discord.Net.Template.Interactions.AutoCompletes;
using Discord.Net.Template.Utility;
using Fergun.Interactive;
using Fergun.Interactive.Pagination;

namespace Discord.Net.Template.Interactions;

[Order(1)]
public class Help : InteractionModuleBase<SocketInteractionContext>
{
    public InteractiveService? Interactive { get; set; }

    [SlashCommand("help", "List of slash commands")]
    public async Task HelpCommand([Autocomplete(typeof(SlashCommandNameAutoComplete))] string? commandName = null)
    {
        if (!string.IsNullOrEmpty(commandName))
        {
            await Command(commandName);

            return;
        }

        var modules = GetSlashCommandModules();

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

        await Interactive!.SendPaginatorAsync(paginator, Context.Interaction, TimeSpan.FromMinutes(10));

        PageBuilder GeneratePage(int index)
        {
            var page = new PageBuilder()
                .WithDefaultColor()
                .WithTitle(modules[index].SlashGroupName ?? modules[index].Name);

            var commands = modules[index].SlashCommands
                .Where(c => !InfoUtility.HaveAttribute<HideInHelpAttribute>(c) && !string.IsNullOrEmpty(c.Description))
                .DistinctBy(c => c.Name)
                .ToList();

            foreach (var command in commands)
            {
                var parameters = string.Join(' ',
                    command.Parameters
                        .Where(p => p.Name != "")
                        .Select(p => p.IsRequired ? $"`{p.Name}`" : $"`[{p.Name}]`"));

                page.AddField(
                    $"/{GetFullCommandName(command)} {parameters}",
                    command.Description.Split('\n')[0]);
            }

            return page;
        }
    }

    public async Task Command(string commandName)
    {
        var modules = GetSlashCommandModules();

        var commands = modules
            .SelectMany(GetSlashCommandsFromModule)
            .Where(c => !InfoUtility.HaveAttribute<HideInHelpAttribute>(c) && !string.IsNullOrEmpty(c.Description))
            .Where(c => c.Name == commandName || GetFullCommandName(c) == commandName)
            .ToList();

        if (!commands.Any())
        {
            await Context.RespondAsync("Cannot find matching commands", true);

            return;
        }

        var embed = new EmbedBuilder()
            .WithDefaultColor()
            .WithTitle($"`{commandName}` commands");

        foreach (var command in commands)
        {
            var parameters = string.Join(' ',
                command.Parameters
                    .Where(p => p.Name != "")
                    .Select(p => $"`{p.Name}`"));

            embed.AddField(
                $"/{GetFullCommandName(command)} {parameters}",
                command.Description);
        }

        await Context.RespondEmbedAsync(embed.Build());
    }

    public static List<ModuleInfo> GetSlashCommandModules()
    {
        List<ModuleInfo> modules = InteractionManager.Service.Modules
            .Where(m => !m.IsSubModule && !InfoUtility.HaveAttribute<HideInHelpAttribute>(m))
            .ToList();
        modules.Sort((m1, m2) => GetOrder(m1).CompareTo(GetOrder(m2)));

        return modules;
    }

    public static List<SlashCommandInfo> GetSlashCommandsFromModule(ModuleInfo module)
    {
        return module.SlashCommands.Concat(module.SubModules.OrderBy(GetOrder).SelectMany(GetSlashCommandsFromModule))
            .ToList();
    }

    private static int GetOrder(ModuleInfo module)
    {
        return InfoUtility.HaveAttribute<OrderAttribute>(module)
            ? InfoUtility.GetAttribute<OrderAttribute>(module).Order
            : int.MaxValue;
    }

    public static string GetFullCommandName(ICommandInfo command)
    {
        return $"{GetParentName(command.Module)} {command.Name}".Trim();
    }

    private static string GetParentName(ModuleInfo? module)
    {
        return module is null ? "" : $"{GetParentName(module.Parent)} {module.SlashGroupName}".Trim();
    }
}
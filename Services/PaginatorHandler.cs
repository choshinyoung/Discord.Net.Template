using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.Net.Template.Attributes;
using Discord.Net.Template.Extensions;
using Discord.Net.Template.Modules.Paginators;
using Microsoft.Extensions.DependencyInjection;

namespace Discord.Net.Template.Services;

public class PaginatorHandler(IServiceProvider services) : IModuleHandler
{
    private Dictionary<string, (MethodInfo method, Type type)> paginators = [];

    public async Task InitializeAsync()
    {
        await LoadModulesAsync();
    }

    public async Task LoadModulesAsync()
    {
        var types = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(IPaginator).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var type in types)
        {
            foreach (var method in type.GetMethods())
            {
                var attr = method.GetCustomAttribute<PaginatorAttribute>();

                if (attr is null)
                {
                    continue;
                }

                paginators.TryAdd(attr.Id, (method, type));
            }
        }
    }

    public async Task UnloadModulesAsync()
    {
        paginators = [];
    }

    public async Task InitPaginator(SocketCommandContext context, string id, int index = 0)
    {
        if (TryBuildPage(id, index, out var embed, out var isLastPage))
        {
            await context.ReplyEmbedAsync(
                embed!,
                component: BuildPageButtons(id, context.User.Id, index, isLastPage)
            );
        }
    }

    public async Task InitPaginator(SocketInteractionContext context, string id, int index = 0)
    {
        if (TryBuildPage(id, index, out var embed, out var isLastPage))
        {
            await context.RespondEmbedAsync(
                embed!,
                component: BuildPageButtons(id, context.User.Id, index, isLastPage)
            );
        }
    }

    public bool TryBuildPage(string id, int index, out Embed? embed, out bool isLastPage)
    {
        embed = null;
        isLastPage = false;

        if (paginators.TryGetValue(id, out var entry))
        {
            if (ActivatorUtilities.CreateInstance(services, entry.type) is not IPaginator paginator)
            {
                return false;
            }

            paginator.Id = id;
            paginator.Index = index;

            if (entry.method.Invoke(paginator, null) is not (Embed _embed, bool _isLastPage))
            {
                return false;
            }

            embed = _embed;
            isLastPage = _isLastPage;

            return true;
        }

        return false;
    }

    public static MessageComponent BuildPageButtons(
        string id,
        ulong userId,
        int index,
        bool isLastPage
    )
    {
        return new ComponentBuilder()
            .WithButton(
                "◀",
                $"paginator:{id}:{userId}:{index - 1}",
                ButtonStyle.Primary,
                disabled: index <= 0
            )
            .WithButton(
                "▶",
                $"paginator:{id}:{userId}:{index + 1}",
                ButtonStyle.Primary,
                disabled: isLastPage
            )
            .Build();
    }
}

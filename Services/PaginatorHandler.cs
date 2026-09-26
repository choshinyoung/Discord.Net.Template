using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.Net.Template.Attributes;
using Discord.Net.Template.Extensions;
using Discord.Net.Template.Modules.Paginators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Discord.Net.Template.Services;

public class PaginatorHandler(IServiceProvider services, ILogger<PaginatorHandler> logger)
    : IModuleHandler
{
    private Dictionary<string, (MethodInfo method, Type type)> paginators = [];

    public async Task InitializeAsync()
    {
        await LoadModulesAsync();
    }

    public async Task LoadModulesAsync()
    {
        var _paginators = new Dictionary<string, (MethodInfo method, Type type)>();

        var types = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(Paginator).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var type in types)
        {
            foreach (var method in type.GetMethods())
            {
                var attr = method.GetCustomAttribute<PaginatorAttribute>();

                if (attr is null)
                {
                    continue;
                }

                if (
                    (
                        method.ReturnType != typeof((Embed, bool))
                        && method.ReturnType != typeof(Task<(Embed, bool)>)
                    )
                    || method.GetParameters().Length != 0
                    || attr.Id.Contains(':')
                )
                {
                    logger.LogWarning(
                        "Invalid paginator method {TypeName}.{MethodName}; skipped.",
                        type.FullName,
                        method.Name
                    );

                    continue;
                }

                _paginators.TryAdd(attr.Id, (method, type));
            }
        }

        paginators = _paginators;

        await Task.CompletedTask;
    }

    public async Task UnloadModulesAsync()
    {
        paginators = [];

        await Task.CompletedTask;
    }

    public async Task InitPaginator(SocketCommandContext context, string id, int index = 0)
    {
        if (await BuildPageAsync(id, index) is ({ } embed, var isLastPage))
        {
            await context.ReplyEmbedAsync(
                embed,
                component: BuildPageButtons(id, context.User.Id, index, isLastPage)
            );
        }
    }

    public async Task InitPaginator(SocketInteractionContext context, string id, int index = 0)
    {
        if (await BuildPageAsync(id, index) is ({ } embed, var isLastPage))
        {
            await context.RespondEmbedAsync(
                embed,
                component: BuildPageButtons(id, context.User.Id, index, isLastPage)
            );
        }
    }

    public async Task<(Embed Embed, bool IsLastPage)?> BuildPageAsync(string id, int index)
    {
        if (!paginators.TryGetValue(id, out var entry))
        {
            return null;
        }

        await using var scope = services.CreateAsyncScope();

        if (
            ActivatorUtilities.CreateInstance(scope.ServiceProvider, entry.type)
            is not Paginator paginator
        )
        {
            return null;
        }

        paginator.Id = id;
        paginator.Index = index;

        return entry.method.Invoke(paginator, null) switch
        {
            Task<(Embed, bool)> task => await task,
            (Embed embed, bool isLastPage) => (embed, isLastPage),
            _ => null,
        };
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

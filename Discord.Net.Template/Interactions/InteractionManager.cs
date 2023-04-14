using System.Reflection;
using Discord.Interactions;
using Discord.Net.Template.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Discord.Net.Template.Interactions;

public static class InteractionManager
{
    private static readonly InteractionServiceConfig InteractionConfig = new()
    {
        DefaultRunMode = RunMode.Async,
        LogLevel = Bot.IsDebugMode ? LogSeverity.Debug : LogSeverity.Info
    };

    public static readonly InteractionService Interaction = new(Bot.Client, InteractionConfig);

    public static readonly IServiceProvider Service = new ServiceCollection()
        .AddSingleton(Bot.Client)
        .AddSingleton(Interaction)
        .BuildServiceProvider();

    public static async Task Initialize()
    {
        await Interaction.AddModulesAsync(Assembly.GetEntryAssembly(), Service);

        InteractionEventHandler.Register();
    }
}
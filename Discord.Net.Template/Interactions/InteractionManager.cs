using System.Reflection;
using Discord.Interactions;
using Discord.Net.Template.Events;

namespace Discord.Net.Template.Interactions;

public static class InteractionManager
{
    private static readonly InteractionServiceConfig InteractionConfig = new()
    {
        DefaultRunMode = RunMode.Async,
        LogLevel = Bot.IsDebugMode ? LogSeverity.Debug : LogSeverity.Info
    };

    public static readonly InteractionService Service = new(Bot.Client, InteractionConfig);

    public static bool IsEnabled { get; private set; }

    public static async Task Initialize()
    {
        IsEnabled = true;

        await LoadModulesAsync();

        InteractionEventHandler.Register();
    }

    public static async Task LoadModulesAsync()
    {
        await Service.AddModulesAsync(Assembly.GetEntryAssembly(), Bot.Service);
    }

    public static async Task UnloadModulesAsync()
    {
        foreach (var module in Service.Modules)
        {
            await Service.RemoveModuleAsync(module);
        }
    }
}
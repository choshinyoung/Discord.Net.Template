using System.Reflection;
using Discord.Commands;
using Discord.Net.Template.Events;
using Discord.Net.Template.Utility;
using Discord.WebSocket;

namespace Discord.Net.Template.Commands;

public static class CommandManager
{
    private static readonly CommandServiceConfig CommandConfig = new()
    {
        DefaultRunMode = RunMode.Async,
        LogLevel = Bot.IsDebugMode ? LogSeverity.Debug : LogSeverity.Info
    };

    public static readonly string Prefix = Config.Get("Prefix");

    public static readonly CommandService Service = new(CommandConfig);

    public static bool IsEnabled { get; private set; }

    public static async Task Initialize()
    {
        IsEnabled = true;

        await LoadModulesAsync();

        CommandEventHandler.Register();
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

    public static async Task ExecuteCommand(SocketUserMessage message)
    {
        SocketCommandContext context = new(Bot.Client, message);

        var argPos = 0;
        if (message.HasStringPrefix(Prefix, ref argPos) ||
            message.HasMentionPrefix(Bot.Client.CurrentUser, ref argPos))
        {
            if (Service.Search(context, argPos).IsSuccess)
            {
                await Service.ExecuteAsync(context, argPos, Bot.Service);
            }
        }
    }
}
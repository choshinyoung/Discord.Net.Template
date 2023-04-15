using System.Reflection;
using Discord.Commands;
using Discord.Net.Template.Events;
using Discord.Net.Template.Utility;

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

    public static async Task Initialize()
    {
        await Service.AddModulesAsync(Assembly.GetEntryAssembly(), Bot.Service);

        CommandEventHandler.Register();
    }
}
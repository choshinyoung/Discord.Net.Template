using System.Reflection;
using Discord.Commands;
using Discord.Net.Template.Events;
using Discord.Net.Template.Utility;
using Microsoft.Extensions.DependencyInjection;

namespace Discord.Net.Template.Commands;

public static class CommandManager
{
    private static readonly CommandServiceConfig CommandConfig = new()
    {
        DefaultRunMode = RunMode.Async,
        LogLevel = Bot.IsDebugMode ? LogSeverity.Debug : LogSeverity.Info
    };

    public static readonly string Prefix = Config.Get("Prefix");

    public static readonly CommandService Command = new(CommandConfig);

    public static readonly IServiceProvider Service = new ServiceCollection()
        .AddSingleton(Bot.Client)
        .AddSingleton(Command)
        .BuildServiceProvider();

    public static async Task Initialize()
    {
        await Command.AddModulesAsync(Assembly.GetEntryAssembly(), Service);

        CommandEventHandler.Register();
    }
}
using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.Net.Template.Extensions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Discord.Net.Template.Services;

public class CommandHandler(
    DiscordSocketClient client,
    IConfiguration config,
    ILogger<CommandHandler> logger,
    IServiceProvider services,
    CommandService command
) : IModuleHandler
{
    public async Task InitializeAsync()
    {
        command.Log += HandleLogAsync;

        client.MessageReceived += HandleMessageReceivedAsync;
        command.CommandExecuted += HandleCommandExecutedAsync;

        await LoadModulesAsync();
    }

    public async Task LoadModulesAsync()
    {
        await command.AddModulesAsync(Assembly.GetEntryAssembly(), services);
    }

    public async Task UnloadModulesAsync()
    {
        foreach (var module in command.Modules)
        {
            await command.RemoveModuleAsync(module);
        }
    }

    private async Task HandleLogAsync(LogMessage message)
    {
        logger.Log(message);

        await Task.CompletedTask;
    }

    private async Task HandleMessageReceivedAsync(SocketMessage message)
    {
        if (
            message is not SocketUserMessage userMessage
            || userMessage.Content == null
            || userMessage.Author.Id == client.CurrentUser.Id
            || userMessage.Author.IsBot
        )
        {
            return;
        }

        await ExecuteCommand(userMessage);
    }

    private async Task HandleCommandExecutedAsync(
        Optional<CommandInfo> command,
        ICommandContext context,
        IResult result
    )
    {
        if (result.IsSuccess)
        {
            return;
        }

        var socketContext = (context as SocketCommandContext)!;

        if (config.GetValue<bool>("Discord:DebugMode"))
        {
            await socketContext.ReplyAsync($"Error Occured!\n```{result.ErrorReason}```");
        }
        else
        {
            await socketContext.AddReactionAsync("⚠️");
        }
    }

    public async Task ExecuteCommand(SocketUserMessage message)
    {
        SocketCommandContext context = new(client, message);

        var argPos = 0;
        if (
            message.HasStringPrefix(config["Discord:Prefix"], ref argPos)
            || message.HasMentionPrefix(client.CurrentUser, ref argPos)
        )
        {
            if (command.Search(context, argPos).IsSuccess)
            {
                await command.ExecuteAsync(context, argPos, services);
            }
        }
    }
}

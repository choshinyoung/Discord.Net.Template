using Discord.Commands;
using Discord.Net.Template.Commands;
using Discord.Net.Template.Extensions;
using Discord.WebSocket;

namespace Discord.Net.Template.Events;

public class CommandEventHandler : IEventHandler
{
    public static void Register()
    {
        CommandManager.Service.Log += OnLog;

        Bot.Client.MessageReceived += OnMessageReceived;
        CommandManager.Service.CommandExecuted += OnServiceExecuted;
    }

    private static async Task OnLog(LogMessage message)
    {
        Console.WriteLine(message);

        await Task.CompletedTask;
    }

    private static async Task OnMessageReceived(SocketMessage message)
    {
        if (message is not SocketUserMessage userMsg || userMsg.Content == null ||
            userMsg.Author.Id == Bot.Client.CurrentUser.Id || userMsg.Author.IsBot)
        {
            return;
        }

        SocketCommandContext context = new(Bot.Client, userMsg);

        var argPos = 0;
        if (userMsg.HasStringPrefix(CommandManager.Prefix, ref argPos) ||
            userMsg.HasMentionPrefix(Bot.Client.CurrentUser, ref argPos))
        {
            if (CommandManager.Service.Search(context, argPos).IsSuccess)
            {
                await CommandManager.Service.ExecuteAsync(context, argPos, Bot.Service);
            }
        }
    }

    private static async Task OnServiceExecuted(Optional<CommandInfo> command, ICommandContext context,
        IResult result)
    {
        if (result.IsSuccess)
        {
            return;
        }

        var socketContext = (context as SocketCommandContext)!;

        if (Bot.IsDebugMode)
        {
            await socketContext.ReplyAsync($"오류 발생!\n```{result.ErrorReason}```");
        }
        else
        {
            await socketContext.AddReactionAsync("⚠️");
        }
    }
}
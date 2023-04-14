using Discord.Commands;
using Discord.Net.Template.Commands;
using Discord.Net.Template.Extensions;
using Discord.Net.Template.Utility;
using Discord.WebSocket;

namespace Discord.Net.Template.Events;

public class CommandEventHandler : IEventHandler
{
    public static void Register()
    {
        CommandManager.Command.Log += OnLog;

        Bot.Client.MessageReceived += OnMessageReceived;
        CommandManager.Command.CommandExecuted += OnCommandExecuted;
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
            if (CommandManager.Command.Search(context, argPos).IsSuccess)
            {
                await CommandManager.Command.ExecuteAsync(context, argPos, CommandManager.Service);
            }
        }
    }

    private static async Task OnCommandExecuted(Optional<CommandInfo> command, ICommandContext context,
        IResult result)
    {
        var socketContext = (context as SocketCommandContext)!;

        if (Bot.IsDebugMode)
        {
            var emb = EmbedUtility.CreateEmbedFromContext(socketContext, title: "오류 발생!",
                description: result.ErrorReason);

            await socketContext.ReplyEmbedAsync(emb.Build());
        }
        else
        {
            IEmote emote = new Emoji("⚠️");

            await context.Message.AddReactionAsync(emote);
        }
    }
}
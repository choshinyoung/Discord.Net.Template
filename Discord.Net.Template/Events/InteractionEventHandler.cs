using Discord.Interactions;
using Discord.Net.Template.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Discord.Net.Template.Events;

public class InteractionEventHandler : IEventHandler
{
    public static void Register()
    {
        InteractionManager.Interaction.Log += OnLog;

        Bot.Client.Ready += OnReady;

        Bot.Client.InteractionCreated += OnInteractionCreated;
        InteractionManager.Interaction.SlashCommandExecuted += OnSlashCommandExecuted;
    }

    private static async Task OnLog(LogMessage message)
    {
        Console.WriteLine(message);

        await Task.CompletedTask;
    }

    private static async Task OnReady()
    {
        await InteractionManager.Interaction.RegisterCommandsGloballyAsync();
    }

    private static async Task OnInteractionCreated(SocketInteraction interaction)
    {
        var scope = InteractionManager.Service.CreateScope();
        SocketInteractionContext ctx = new(Bot.Client, interaction);

        await InteractionManager.Interaction.ExecuteCommandAsync(ctx, scope.ServiceProvider);
    }

    private static async Task OnSlashCommandExecuted(SlashCommandInfo command, IInteractionContext context,
        IResult result)
    {
        if (result is { IsSuccess: false, Error: InteractionCommandError.UnmetPrecondition })
        {
            await context.Interaction.RespondAsync("권한이 없어서 커맨드를 실행할 수 없어요", ephemeral: true);
        }
    }
}
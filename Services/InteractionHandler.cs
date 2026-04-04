using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.Net.Template.Attributes;
using Discord.Net.Template.Extensions;
using Discord.Net.Template.Utils;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Discord.Net.Template.Services;

public class InteractionHandler(
    DiscordSocketClient client,
    IConfiguration config,
    ILogger<InteractionHandler> logger,
    IServiceProvider services,
    InteractionService interaction
) : IModuleHandler
{
    public async Task InitializeAsync()
    {
        interaction.Log += HandleLogAsync;
        client.Ready += HandleReadyAsync;

        client.InteractionCreated += HandleInteractionCreatedAsync;
        interaction.SlashCommandExecuted += HandleSlashCommandExecutionAsync;

        await LoadModulesAsync();
    }

    public async Task LoadModulesAsync()
    {
        await interaction.AddModulesAsync(Assembly.GetEntryAssembly(), services);
    }

    public async Task UnloadModulesAsync()
    {
        foreach (var module in interaction.Modules)
        {
            await interaction.RemoveModuleAsync(module);
        }
    }

    private async Task HandleLogAsync(LogMessage message)
    {
        logger.Log(message);

        await Task.CompletedTask;
    }

    private async Task HandleReadyAsync()
    {
        await interaction.RegisterCommandsGloballyAsync();
    }

    private async Task HandleInteractionCreatedAsync(SocketInteraction intr)
    {
        SocketInteractionContext ctx = new(client, intr);
        await interaction.ExecuteCommandAsync(ctx, services);
    }

    private async Task HandleSlashCommandExecutionAsync(
        SlashCommandInfo command,
        IInteractionContext context,
        IResult result
    )
    {
        if (result.IsSuccess)
        {
            return;
        }

        var socketContext = (context as SocketInteractionContext)!;

        if (result is { IsSuccess: false, Error: InteractionCommandError.UnmetPrecondition })
        {
            await socketContext.RespondAsync(
                "You don't have permission to execute this command.",
                true
            );

            return;
        }

        if (config.GetSection("Discord:DebugMode").Get<bool>())
        {
            await socketContext.RespondOrFollowupAsync(
                $"Error Occured!\n```{result.ErrorReason}```",
                true
            );
        }
        else
        {
            await socketContext.RespondOrFollowupAsync("Error Occured!", true);
        }
    }
}

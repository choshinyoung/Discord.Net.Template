using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.Net.Template.Extensions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
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
        interaction.SlashCommandExecuted += HandleCommandExecutionAsync;
        interaction.ComponentCommandExecuted += HandleCommandExecutionAsync;
        interaction.AutocompleteHandlerExecuted += HandleAutocompleteExecutionAsync;

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

    public async Task RegisterCommandsAsync()
    {
        var testGuildId = config.GetValue<ulong>("Discord:TestGuildId");

        if (config.GetValue<bool>("Discord:DebugMode") && testGuildId != 0)
        {
            await interaction.RegisterCommandsToGuildAsync(testGuildId);
        }
        else
        {
            await interaction.RegisterCommandsGloballyAsync();
        }
    }

    private async Task HandleReadyAsync()
    {
        client.Ready -= HandleReadyAsync;

        await RegisterCommandsAsync();
    }

    private async Task HandleInteractionCreatedAsync(SocketInteraction intr)
    {
        SocketInteractionContext ctx = new(client, intr);
        await interaction.ExecuteCommandAsync(ctx, services);
    }

    private async Task HandleCommandExecutionAsync(
        ICommandInfo? command,
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
            await socketContext.RespondOrFollowupAsync(
                "You don't have permission to execute this command.",
                true
            );

            return;
        }

        if (config.GetValue<bool>("Discord:DebugMode"))
        {
            await socketContext.RespondOrFollowupAsync(
                $"Error Occurred!\n```{result.ErrorReason}```",
                true
            );
        }
        else
        {
            await socketContext.RespondOrFollowupAsync("Error Occurred!", true);
        }
    }

    private Task HandleAutocompleteExecutionAsync(
        IAutocompleteHandler handler,
        IInteractionContext context,
        IResult result
    )
    {
        if (!result.IsSuccess)
        {
            logger.LogWarning(
                "Autocomplete handler {Handler} failed: {Error}",
                handler.GetType().Name,
                result.ErrorReason
            );
        }

        return Task.CompletedTask;
    }
}

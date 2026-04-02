using System.Reflection.Metadata;
using Discord.Interactions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: MetadataUpdateHandler(typeof(Discord.Net.Template.Services.HotReloadHandler))]

namespace Discord.Net.Template.Services;

public class HotReloadHandler
{
    public static IServiceProvider? ServiceProvider { get; set; }

    public static void UpdateApplication(Type[]? types)
    {
        if (ServiceProvider is null)
        {
            return;
        }

        _ = ReloadModules(ServiceProvider);
    }

    private static async Task ReloadModules(IServiceProvider services)
    {
        var handlers = services.GetRequiredService<IEnumerable<IModuleHandler>>();
        var logger = services.GetRequiredService<ILogger<HotReloadHandler>>();

        foreach (var handler in handlers)
        {
            await handler.UnloadModulesAsync();
            await handler.LoadModulesAsync();
        }

        var interactionService = services.GetRequiredService<InteractionService>();
        await interactionService.RegisterCommandsGloballyAsync();

        logger.LogInformation("Hot Reload completed successfully.");
    }
}

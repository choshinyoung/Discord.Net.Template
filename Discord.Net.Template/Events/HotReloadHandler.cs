using System.Reflection.Metadata;
using Discord.Net.Template.Commands;
using Discord.Net.Template.Events;
using Discord.Net.Template.Interactions;

[assembly: MetadataUpdateHandler(typeof(HotReloadHandler))]

namespace Discord.Net.Template.Events;

public class HotReloadHandler
{
    public static void UpdateApplication(Type[]? types)
    {
        Task.Run(async () => await ReloadModules());
    }

    private static async Task ReloadModules()
    {
        if (InteractionManager.IsEnabled)
        {
            await InteractionManager.UnloadModulesAsync();
            await InteractionManager.LoadModulesAsync();

            await InteractionManager.Service.RegisterCommandsGloballyAsync();
        }

        if (CommandManager.IsEnabled)
        {
            await CommandManager.UnloadModulesAsync();
            await CommandManager.LoadModulesAsync();
        }

        Console.WriteLine("Reload complete.");
    }
}
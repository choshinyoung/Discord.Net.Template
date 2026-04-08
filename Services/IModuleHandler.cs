namespace Discord.Net.Template.Services;

public interface IModuleHandler
{
    Task InitializeAsync();

    Task LoadModulesAsync();

    Task UnloadModulesAsync();
}

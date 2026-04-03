using Discord.Interactions;
using Discord.Net.Template.Attributes;
using Discord.Net.Template.Extensions;
using Discord.Net.Template.Services;
using Discord.Net.Template.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace Discord.Net.Template.Modules.Interactions.AutoCompletes;

public class HelpAutoComplete : AutocompleteHandler
{
    public override Task<AutocompletionResult> GenerateSuggestionsAsync(
        IInteractionContext context,
        IAutocompleteInteraction autocompleteInteraction,
        IParameterInfo parameter,
        IServiceProvider services
    )
    {
        var interactionHandler = services.GetRequiredService<InteractionHandler>();
        var modules = interactionHandler.GetModules();

        var commands = modules
            .SelectMany(x => x.GetCommands())
            .Where(c =>
                !InfoUtil.HaveAttribute<HideInHelpAttribute>(c)
                && !string.IsNullOrEmpty(c.Description)
            )
            .DistinctBy(x => x.GetFullName())
            .ToList();

        return Task.FromResult(
            AutocompletionResult.FromSuccess(
                commands.Select(c => new AutocompleteResult(c.GetFullName(), c.GetFullName()))
            )
        );
    }
}

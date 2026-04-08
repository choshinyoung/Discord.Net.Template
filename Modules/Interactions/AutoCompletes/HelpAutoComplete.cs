using Discord;
using Discord.Interactions;
using Discord.Net.Template.Attributes;
using Discord.Net.Template.Extensions;
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
        var interaction = services.GetRequiredService<InteractionService>();
        var modules = interaction.GetModules();

        var userInput = autocompleteInteraction.Data.Current.Value?.ToString() ?? "";

        var suggestions = modules
            .SelectMany(x => x.GetCommands())
            .Where(x =>
                !InfoUtil.HaveAttribute<HideInHelpAttribute>(x)
                && !string.IsNullOrEmpty(x.Description)
                && x.Name.Contains(userInput, StringComparison.OrdinalIgnoreCase)
            )
            .DistinctBy(x => x.GetFullName())
            .ToList();

        return Task.FromResult(
            AutocompletionResult.FromSuccess(
                suggestions.Select(c => new AutocompleteResult(c.GetFullName(), c.GetFullName()))
            )
        );
    }
}

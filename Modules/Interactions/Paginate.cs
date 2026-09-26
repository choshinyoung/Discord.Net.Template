using Discord.Interactions;
using Discord.Net.Template.Extensions;
using Discord.Net.Template.Services;
using Discord.WebSocket;

namespace Discord.Net.Template.Modules.Interactions;

public class Paginate(PaginatorHandler paginator) : InteractionModuleBase<SocketInteractionContext>
{
    [ComponentInteraction("paginator:*:*:*")]
    public async Task UpdatePaginator(string id, ulong ownerId, int index)
    {
        if (Context.User.Id != ownerId)
        {
            await Context.RespondAsync(
                "Only the user who ran this command can use these buttons.",
                true
            );

            return;
        }

        if (
            index < 0
            || Context.Interaction is not SocketMessageComponent component
            || !paginator.TryBuildPage(id, index, out var embed, out var isLastPage)
        )
        {
            await DeferAsync();

            return;
        }

        await component.UpdateAsync(x =>
        {
            x.Embed = embed;
            x.Components = PaginatorHandler.BuildPageButtons(id, ownerId, index, isLastPage);
        });
    }
}

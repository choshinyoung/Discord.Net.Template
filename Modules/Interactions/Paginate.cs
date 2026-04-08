using Discord.Interactions;
using Discord.Net.Template.Services;
using Discord.WebSocket;

namespace Discord.Net.Template.Modules.Interactions;

public class Paginate(PaginatorHandler paginator) : InteractionModuleBase<SocketInteractionContext>
{
    [ComponentInteraction("paginator:*:*:*")]
    public async Task UpdatePaginator(string id, ulong ownerId, int index)
    {
        if (Context.User.Id != ownerId || index < 0)
        {
            return;
        }

        if (paginator.TryBuildPage(id, index, out var embed, out var isLastPage))
        {
            if (Context.Interaction is not SocketMessageComponent component)
            {
                return;
            }

            await component.UpdateAsync(x =>
            {
                x.Embed = embed;
                x.Components = PaginatorHandler.BuildPageButtons(id, ownerId, index, isLastPage);
            });
        }
    }
}

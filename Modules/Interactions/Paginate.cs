using Discord.Interactions;
using Discord.Net.Template.Services;
using Discord.WebSocket;

namespace Discord.Net.Template.Modules.Interactions;

public class Paginate(PaginatorHandler paginator) : InteractionModuleBase<SocketInteractionContext>
{
    [ComponentInteraction("paginator:*:*:*")]
    public async Task UpdatePaginator(string id, string userId, string pageIndex)
    {
        if (
            !ulong.TryParse(userId, out var ownerId)
            || !int.TryParse(pageIndex, out var targetIndex)
        )
        {
            return;
        }

        if (Context.User.Id != ownerId)
        {
            return;
        }

        if (paginator.TryBuildPage(id, targetIndex, out var embed, out var isLastPage))
        {
            if (Context.Interaction is not SocketMessageComponent component)
            {
                return;
            }

            await component.UpdateAsync(x =>
            {
                x.Embed = embed;
                x.Components = PaginatorHandler.BuildPageButtons(
                    id,
                    ownerId,
                    targetIndex,
                    isLastPage
                );
            });
        }
    }
}

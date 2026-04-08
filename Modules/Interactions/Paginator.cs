using Discord.Interactions;

namespace Discord.Net.Template.Modules.Interactions;

public class Paginator() : InteractionModuleBase<SocketInteractionContext>
{
    [ComponentInteraction("paginator:*:*:*")]
    public async Task UpdatePaginator(string paginatorId, string userId, string pageIndex)
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
    }
}

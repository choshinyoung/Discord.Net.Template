using Discord.Interactions;
using Discord.Net.Template.Utility;

namespace Discord.Net.Template.Extensions;

public static class SocketInteractionContextExtensions
{
    public static async Task RespondAsync(this SocketInteractionContext context, object content,
        bool ephemeral = false, bool disableMention = true, MessageComponent? component = null)
    {
        await context.Interaction.RespondAsync(content.ToString(), ephemeral: ephemeral,
            allowedMentions: disableMention ? AllowedMentions.None : null, components: component);
    }

    public static async Task FollowupAsync(this SocketInteractionContext context, object content,
        bool ephemeral = false, bool disableMention = true, MessageComponent? component = null)
    {
        await context.Interaction.FollowupAsync(content.ToString(), ephemeral: ephemeral,
            allowedMentions: disableMention ? AllowedMentions.None : null, components: component);
    }

    public static async Task SendAsync(this SocketInteractionContext context, object content,
        bool disableMention = true, MessageComponent? component = null, MessageReference? messageReference = null)
    {
        await context.Channel.SendMessageAsync(content.ToString(),
            allowedMentions: disableMention ? AllowedMentions.None : null, components: component,
            messageReference: messageReference);
    }

    public static async Task RespondEmbedAsync(this SocketInteractionContext context, object content,
        bool ephemeral = false, bool disableMention = true, MessageComponent? component = null)
    {
        var emb = EmbedUtility.CreateEmbedFromContext(context, content.ToString()).Build();

        await context.Interaction.RespondAsync(embed: emb, ephemeral: ephemeral,
            allowedMentions: disableMention ? AllowedMentions.None : null, components: component);
    }

    public static async Task RespondEmbedAsync(this SocketInteractionContext context, Embed emb, string? content = null,
        bool ephemeral = false, bool disableMention = true, MessageComponent? component = null)
    {
        await context.Interaction.RespondAsync(content, embed: emb, ephemeral: ephemeral,
            allowedMentions: disableMention ? AllowedMentions.None : null, components: component);
    }
}
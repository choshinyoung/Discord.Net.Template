using Discord.Commands;
using Discord.Net.Template.Utility;
using Discord.Rest;

namespace Discord.Net.Template.Extensions;

public static class SocketCommandContextExtensions
{
    public static async Task<RestUserMessage> ReplyAsync(this SocketCommandContext context, object content,
        bool disableMention = true, MessageComponent? component = null)
    {
        var reference = await context.Channel.GetMessageAsync(context.Message.Id) is not null
            ? new MessageReference(context.Message.Id, context.Channel.Id, context.Guild?.Id)
            : null;

        return await context.Channel.SendMessageAsync(content.ToString(),
            allowedMentions: disableMention ? AllowedMentions.None : null,
            messageReference: reference, components: component);
    }

    public static async Task<RestUserMessage> ReplyEmbedAsync(this SocketCommandContext context, object content,
        bool disableMention = true, MessageComponent? component = null)
    {
        var reference = await context.Channel.GetMessageAsync(context.Message.Id) is not null
            ? new MessageReference(context.Message.Id, context.Channel.Id, context.Guild?.Id)
            : null;

        var emb = EmbedUtility.CreateEmbedFromContext(context, content.ToString()).Build();

        return await context.Channel.SendMessageAsync(embed: emb,
            allowedMentions: disableMention ? AllowedMentions.None : null,
            messageReference: reference, components: component);
    }

    public static async Task<RestUserMessage> ReplyEmbedAsync(this SocketCommandContext context, Embed emb,
        bool disableMention = true, MessageComponent? component = null)
    {
        var reference = await context.Channel.GetMessageAsync(context.Message.Id) is not null
            ? new MessageReference(context.Message.Id, context.Channel.Id, context.Guild?.Id)
            : null;

        return await context.Channel.SendMessageAsync(embed: emb,
            allowedMentions: disableMention ? AllowedMentions.None : null,
            messageReference: reference, components: component);
    }
}
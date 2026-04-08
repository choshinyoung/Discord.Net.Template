using Discord;
using Discord.Commands;
using Discord.Rest;

namespace Discord.Net.Template.Extensions;

public static class SocketCommandContextExtensions
{
    public static async Task<RestUserMessage> ReplyAsync(
        this SocketCommandContext context,
        object content,
        bool disableMention = true,
        MessageComponent? component = null
    )
    {
        return await context.Channel.SendMessageAsync(
            content.ToString(),
            allowedMentions: disableMention ? Discord.AllowedMentions.None : null,
            messageReference: new MessageReference(context.Message.Id),
            components: component
        );
    }

    public static async Task<RestUserMessage> ReplyAsFileAsync(
        this SocketCommandContext context,
        object? title,
        object content,
        bool disableMention = true,
        MessageComponent? component = null
    )
    {
        return await context.Channel.SendFileAsync(
            content.ToString()!.ToStream(),
            "message.txt",
            title?.ToString(),
            allowedMentions: disableMention ? AllowedMentions.None : null,
            messageReference: new MessageReference(context.Message.Id),
            components: component
        );
    }

    public static async Task<RestUserMessage> ReplyAsFileAsync(
        this SocketCommandContext context,
        object content,
        bool disableMention = true,
        MessageComponent? component = null
    )
    {
        if (content.ToString()!.Length <= 2000)
        {
            return await ReplyAsync(context, content, disableMention, component);
        }

        return await ReplyAsFileAsync(context, null, content, disableMention, component);
    }

    public static async Task<RestUserMessage> ReplyEmbedAsync(
        this SocketCommandContext context,
        object content,
        bool disableMention = true,
        MessageComponent? component = null
    )
    {
        var embed = new EmbedBuilder()
            .WithDefaultColor()
            .WithDescription(content.ToString())
            .Build();

        return await context.Channel.SendMessageAsync(
            embed: embed,
            allowedMentions: disableMention ? AllowedMentions.None : null,
            messageReference: new MessageReference(context.Message.Id),
            components: component
        );
    }

    public static async Task<RestUserMessage> ReplyEmbedAsync(
        this SocketCommandContext context,
        Embed embed,
        bool disableMention = true,
        MessageComponent? component = null
    )
    {
        return await context.Channel.SendMessageAsync(
            embed: embed,
            allowedMentions: disableMention ? AllowedMentions.None : null,
            messageReference: new MessageReference(context.Message.Id),
            components: component
        );
    }

    public static async Task AddReactionAsync(this SocketCommandContext context, string emote)
    {
        if (Emote.TryParse(emote, out var parsedEmote))
        {
            await context.Message.AddReactionAsync(parsedEmote);
        }
        else
        {
            await context.Message.AddReactionAsync(new Emoji(emote));
        }
    }
}

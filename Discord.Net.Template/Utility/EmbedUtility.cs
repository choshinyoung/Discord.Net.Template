using Discord.Commands;
using Discord.Interactions;
using Discord.Net.Template.Extensions;
using Discord.WebSocket;

namespace Discord.Net.Template.Utility;

public static class EmbedUtility
{
    public static EmbedBuilder CreateEmbedFromContext(SocketInteractionContext context, object? description = null,
        string? title = null, string? imgUrl = null, string? url = null, string? thumbnailUrl = null,
        Color? color = null)
    {
        return CreateEmbedFromUser(context.User, description, title, imgUrl, url, thumbnailUrl, color);
    }

    public static EmbedBuilder CreateEmbedFromContext(SocketCommandContext context, object? description = null,
        string? title = null, string? imgUrl = null, string? url = null, string? thumbnailUrl = null,
        Color? color = null)
    {
        return CreateEmbedFromUser(context.User, description, title, imgUrl, url, thumbnailUrl, color);
    }

    public static EmbedBuilder CreateEmbedFromUser(SocketUser user, object? description = null, string? title = null,
        string? imgUrl = null, string? url = null, string? thumbnailUrl = null, Color? color = null)
    {
        return CreateEmbed(description, title, imgUrl, url, thumbnailUrl, color, new EmbedFooterBuilder
        {
            Text = user.GetName(),
            IconUrl = user.GetAvatar()
        });
    }

    public static EmbedBuilder CreateEmbed(object? description = null, string? title = null, string? imgUrl = null,
        string? url = null, string? thumbnailUrl = null, Color? color = null, EmbedFooterBuilder? footer = null)
    {
        return new EmbedBuilder
        {
            Title = title,
            Color = color ?? new Color(170, 244, 255),
            Description = description?.ToString(),
            ImageUrl = imgUrl,
            Url = url,
            ThumbnailUrl = thumbnailUrl,
            Footer = footer
        };
    }

    public static EmbedBuilder AddEmptyField(this EmbedBuilder emb)
    {
        return emb.AddField("**  **", "** **", true);
    }
}
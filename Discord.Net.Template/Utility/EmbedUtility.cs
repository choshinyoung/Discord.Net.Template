using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive;

namespace Discord.Net.Template.Utility;

public static class EmbedUtility
{
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

    public static PageBuilder CreatePage(SocketCommandContext context, object? description = null,
        string? title = null, string? imgUrl = null, string? url = null, string? thumbnailUrl = null,
        Color? color = null)
    {
        return CreatePage(context.User, description, title, imgUrl, url, thumbnailUrl, color);
    }

    public static PageBuilder CreatePage(SocketInteractionContext context, object? description = null,
        string? title = null, string? imgUrl = null, string? url = null, string? thumbnailUrl = null,
        Color? color = null)
    {
        return CreatePage(context.User, description, title, imgUrl, url, thumbnailUrl, color);
    }

    public static PageBuilder CreatePage(SocketUser user, object? description = null, string? title = null,
        string? imgUrl = null, string? url = null, string? thumbnailUrl = null, Color? color = null)
    {
        return new PageBuilder
        {
            Title = title ?? "",
            Color = color ?? new Color(170, 244, 255),
            // Footer = new EmbedFooterBuilder
            // {
            //     Text = user.GetName(),
            //     IconUrl = user.GetAvatar()
            // },
            Description = description?.ToString() ?? "",
            ImageUrl = imgUrl ?? "",
            Url = url ?? "",
            ThumbnailUrl = thumbnailUrl ?? ""
        };
    }
}
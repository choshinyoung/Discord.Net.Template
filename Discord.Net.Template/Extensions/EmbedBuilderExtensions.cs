using Fergun.Interactive;

namespace Discord.Net.Template.Extensions;

public static class EmbedBuilderExtensions
{
    public static readonly Color DefaultEmbedColor = new(170, 244, 255);

    public static EmbedBuilder AddEmptyField(this EmbedBuilder embed)
    {
        return embed.AddField("**  **", "** **", true);
    }

    public static EmbedBuilder WithDefaultColor(this EmbedBuilder embed)
    {
        return embed.WithColor(DefaultEmbedColor);
    }

    public static PageBuilder WithDefaultColor(this PageBuilder page)
    {
        return page.WithColor(DefaultEmbedColor);
    }
}
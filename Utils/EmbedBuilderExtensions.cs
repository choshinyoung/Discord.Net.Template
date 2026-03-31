using Discord;

namespace Discord.Net.Template.Utils;

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
}

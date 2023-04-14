using Discord.WebSocket;

namespace Discord.Net.Template.Extensions;

public static class DiscordUserExtension
{
    public static string GetAvatar(this IUser user)
    {
        if (user is not SocketGuildUser guildUser)
        {
            return user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl();
        }

        if (guildUser.GetGuildAvatarUrl() is { } avatar)
        {
            return avatar;
        }

        return user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl();
    }

    public static string GetName(this IUser user)
    {
        return (user as SocketGuildUser)?.Nickname ?? user.Username;
    }
}
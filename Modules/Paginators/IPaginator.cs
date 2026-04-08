using Discord;

namespace Discord.Net.Template.Modules.Paginators;

public abstract class IPaginator
{
    public required string Id { get; set; }
    public int Index { get; set; }
}

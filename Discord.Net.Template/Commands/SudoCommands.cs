using System.Diagnostics;
using Discord.Commands;
using Discord.Net.Template.Extensions;
using Discord.Net.Template.Utility;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Discord.Net.Template.Commands;

[Group("sudo")]
[RequireOwner]
public class SudoCommands : ModuleBase<SocketCommandContext>
{
    [Command("run")]
    [Alias("eval", "execute")]
    public async Task Execute([Remainder] string code)
    {
        var regex = StringUtility.CodeRegex();

        var match = regex.Match(code);

        var trimmedCode = match.Groups["block_code"].Value is var block and not ""
            ? block
            : match.Groups["code"].Value;

        var assembly = typeof(Program).Assembly;

        try
        {
            var result = await CSharpScript.EvaluateAsync(trimmedCode,
                ScriptOptions.Default.WithReferences(assembly).WithImports(
                    "System", "System.Collections.Generic", "System.IO", "System.Linq", "System.Net.Http",
                    "System.Threading", "System.Threading.Tasks", "System.Diagnostics",
                    "System.Text", "System.Text.RegularExpressions", "System.Net", "System.Numerics",
                    "Discord", "Discord.Commands", "Discord.Interactions",
                    "Discord.WebSocket", "Discord.Rest", "Discord.Net",
                    "Newtonsoft.Json", "Discord.Net.Template", "Discord.Net.Template.Commands",
                    "Discord.Net.Template.Interactions",
                    "Discord.Net.Template.Events", "Discord.Net.Template.Extensions", "Discord.Net.Template.Utility"
                ), this);

            if (result is not null)
            {
                await Context.ReplyAsync(result);
            }

            await Context.AddReactionAsync("✅");
        }
        catch (Exception e)
        {
            await Context.ReplyAsFileAsync($"오류 발생!\n```{e}```");
        }
    }

    [Command("status")]
    [Alias("info")]
    public async Task Status()
    {
        var process = Process.GetCurrentProcess();

        var embed = EmbedUtility.CreateEmbedFromContext(Context, title: $"{Bot.Client.CurrentUser.Username} Status");

        embed.AddField("Version", Config.Get("VERSION"), true);
        embed.AddField("Uptime", $"<t:{((DateTimeOffset)process.StartTime).ToUnixTimeSeconds()}:R>", true);
        embed.AddEmptyField();

        embed.AddField("Memory Usage", $"{process.WorkingSet64 * 1e-6:##.##}MB", true);
        embed.AddField("Latency", $"{Bot.Client.Latency}ms", true);

        embed.AddField("Servers", $"{Bot.Client.Guilds.Count} servers");

        await Context.ReplyEmbedAsync(embed.Build());
    }

    [Command("restart")]
    public async Task Restart()
    {
    }
}
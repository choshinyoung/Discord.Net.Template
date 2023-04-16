using System.Text.RegularExpressions;
using Discord.Commands;
using Discord.Net.Template.Extensions;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Discord.Net.Template.Commands;

[Group("sudo")]
[RequireOwner]
public partial class SudoCommands : ModuleBase<SocketCommandContext>
{
    [Command("run")]
    [Alias("eval", "execute")]
    public async Task Execute([Remainder] string code)
    {
        var regex = CodeRegex();

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
                    "System.Threading", "System.Threading.Tasks",
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

    [GeneratedRegex("^\\s*(```(cs)?\\s*(?<block_code>.+)\\s*```)|(?<code>.+)\\s*$")]
    private static partial Regex CodeRegex();
}
using System.Diagnostics;
using System.Reflection;
using Discord.Commands;
using Discord.Net.Template.Attributes;
using Discord.Net.Template.Extensions;
using Discord.Net.Template.Interactions;
using Discord.Net.Template.Utility;
using Discord.WebSocket;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Discord.Net.Template.Commands;

[Group("sudo")]
[Order(2)]
[RequireOwner]
public class SudoCommands : ModuleBase<SocketCommandContext>
{
    [Command("run")]
    [Alias("eval", "execute")]
    [Summary("Runs C# code")]
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
                    "System.Threading", "System.Threading.Tasks", "System.Diagnostics", "System.Reflection",
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
            await Context.ReplyAsFileAsync($"Error Occured!\n```{e}```");
        }
    }

    [Command("status")]
    [Alias("info")]
    [Summary("Checks the bots information")]
    public async Task Status()
    {
        var process = Process.GetCurrentProcess();

        var embed = EmbedUtility.CreateEmbed(Context, $"{Bot.Client.CurrentUser.Username} Status");

        embed.AddField("Version", Config.Get("VERSION"), true);
        embed.AddField("Uptime", $"<t:{((DateTimeOffset)process.StartTime).ToUnixTimeSeconds()}:R>", true);
        embed.AddEmptyField();

        embed.AddField("Memory Usage", $"{process.WorkingSet64 * 1e-6:##.##}MB", true);
        embed.AddField("Latency", $"{Bot.Client.Latency}ms", true);

        embed.AddField("Servers", $"{Bot.Client.Guilds.Count} servers");

        await Context.ReplyEmbedAsync(embed.Build());
    }

    [Command("su")]
    [Summary("Simulates a command as if the targeted user is using it.")]
    public async Task Su(SocketUser user, [Remainder] string command)
    {
        var message = Context.Message;

        var authorProperty = typeof(SocketMessage).GetField("<Author>k__BackingField",
            BindingFlags.Instance | BindingFlags.NonPublic)!;
        authorProperty.SetValue(message, user);

        var contentProperty = typeof(SocketMessage).GetField("<Content>k__BackingField",
            BindingFlags.Instance | BindingFlags.NonPublic)!;
        contentProperty.SetValue(message, command);

        await CommandManager.ExecuteCommand(message);
    }

    [Command("reload")]
    [Summary("Reloads command modules")]
    public async Task Reload()
    {
        if (InteractionManager.IsEnabled)
        {
            await InteractionManager.UnloadModulesAsync();
            await InteractionManager.LoadModulesAsync();

            await InteractionManager.Service.RegisterCommandsGloballyAsync();
        }

        if (CommandManager.IsEnabled)
        {
            await CommandManager.UnloadModulesAsync();
            await CommandManager.LoadModulesAsync();
        }

        await Context.ReplyAsync("Reload complete.");
    }

    [Command("restart")]
    [Summary("Restarts program")]
    public async Task Restart()
    {
        await Context.ReplyAsync("Restarting...");

        Process.Start(Environment.ProcessPath!);
        Environment.Exit(0);
    }
}
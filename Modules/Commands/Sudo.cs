using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Discord;
using Discord.Commands;
using Discord.Net.Template.Attributes;
using Discord.Net.Template.Extensions;
using Discord.Net.Template.Services;
using Discord.Net.Template.Utils;
using Discord.WebSocket;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.Configuration;

namespace Discord.Net.Template.Modules.Commands;

[Group("sudo")]
[Order(2)]
[RequireOwner]
public class SudoCommands(
    DiscordSocketClient client,
    IConfiguration config,
    IEnumerable<IModuleHandler> handlers,
    Discord.Interactions.InteractionService interaction
) : ModuleBase<SocketCommandContext>
{
    [Command("run")]
    [Alias("eval", "execute")]
    [Summary("Runs C# code")]
    public async Task Execute([Remainder] string code)
    {
        var regex = RegexUtil.CodeRegex();
        var match = regex.Match(code);

        var trimmedCode = match.Groups["block_code"].Value is var block and not ""
            ? block
            : match.Groups["code"].Value;

        var assembly = typeof(Program).Assembly;

        try
        {
            var result = await CSharpScript.EvaluateAsync(
                trimmedCode,
                ScriptOptions
                    .Default.WithReferences(assembly)
                    .WithImports(
                        "System",
                        "System.Collections.Generic",
                        "System.IO",
                        "System.Linq",
                        "System.Threading.Tasks",
                        "System.Text",
                        "Discord",
                        "Discord.Commands",
                        "Discord.Interactions",
                        "Discord.WebSocket",
                        "Discord.Rest",
                        "Discord.Net",
                        "Newtonsoft.Json",
                        "Discord.Net.Template",
                        "Discord.Net.Template.Extensions",
                        "Discord.Net.Template.Utils"
                    ),
                this
            );

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

        var embed = new EmbedBuilder()
            .WithDefaultColor()
            .WithDescription($"{client.CurrentUser.Username} Status");

        embed.AddField("Version", config["Version"], true);
        embed.AddField(
            "Uptime",
            $"<t:{((DateTimeOffset)process.StartTime).ToUnixTimeSeconds()}:R>",
            true
        );
        embed.AddEmptyField();

        embed.AddField("Memory Usage", $"{process.WorkingSet64 * 1e-6:##.##}MB", true);
        embed.AddField("Latency", $"{client.Latency}ms", true);

        embed.AddField("Servers", $"{client.Guilds.Count} servers");

        await Context.ReplyEmbedAsync(embed.Build());
    }

    [Command("sh")]
    [Alias("shell", "bash", "cmd")]
    [Summary("Executes command line on terminal")]
    public async Task Shell([Remainder] string commandLine)
    {
        var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        using var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = isWindows ? "cmd.exe" : "bash",
                Arguments = isWindows ? $"/c {commandLine}" : string.Empty,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            },
        };
        process.Start();

        if (!isWindows)
        {
            await process.StandardInput.WriteLineAsync(commandLine);
            await process.StandardInput.FlushAsync();

            process.StandardInput.Close();
        }

        await process.WaitForExitAsync();

        var result = await process.StandardOutput.ReadToEndAsync();

        await Context.ReplyAsFileAsync($"```{result}```");
    }

    [Command("su")]
    [Summary("Simulates a command as if the targeted user is using it.")]
    public async Task Su(SocketUser user, [Remainder] string command)
    {
        var message = Context.Message;

        var authorProperty = typeof(SocketMessage).GetField(
            "<Author>k__BackingField",
            BindingFlags.Instance | BindingFlags.NonPublic
        )!;
        authorProperty.SetValue(message, user);

        var contentProperty = typeof(SocketMessage).GetField(
            "<Content>k__BackingField",
            BindingFlags.Instance | BindingFlags.NonPublic
        )!;
        contentProperty.SetValue(message, command);

        if (handlers.First(x => x is CommandHandler) is not CommandHandler commandHandler)
        {
            return;
        }

        await commandHandler.ExecuteCommand(message);
    }

    [Command("reload")]
    [Summary("Reloads command modules")]
    public async Task Reload()
    {
        foreach (var handler in handlers)
        {
            await handler.UnloadModulesAsync();
            await handler.LoadModulesAsync();
        }

        await interaction.RegisterCommandsGloballyAsync();

        await Context.ReplyAsync("Reload complete.");
    }
}

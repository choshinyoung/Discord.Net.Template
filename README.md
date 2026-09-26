# Discord.Net.Template

[Discord.Net](https://github.com/discord-net/Discord.Net) Template project for .NET 10

## Getting Started

### 1. Install the template

```sh
git clone https://github.com/choshinyoung/Discord.Net.Template.git
cd Discord.Net.Template
dotnet new install .
```

### 2. Create a project

```sh
dotnet new dnettemplate -n MyBot
```

| option            | default | description                                                                   |
|-------------------|---------|-------------------------------------------------------------------------------|
| `--prefix`        | `!`     | Prefix for text commands                                                      |
| `--text-commands` | `true`  | Include prefix-based text commands (requires the Message Content intent)      |
| `--sudo`          | `true`  | Include owner-only `sudo` commands (run C# code, shell, ...). Requires text commands |

For example, a slash-command-only bot:

```sh
dotnet new dnettemplate -n MyBot --text-commands false
```

### 3. Set up the bot in the Discord Developer Portal

1. Create an application at the [Discord Developer Portal](https://discord.com/developers/applications) and copy the bot token.
2. If you kept text commands, enable **Message Content Intent** under **Bot → Privileged Gateway Intents**.
3. Invite the bot with the `bot` and `applications.commands` scopes. The template uses these permissions:
   Send Messages, Embed Links, Attach Files, Add Reactions, Read Message History.

### 4. Configure the token

For local development, use [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets). They're stored outside the project folder and loaded when running with the included launch profile (`DOTNET_ENVIRONMENT=Development`):

```sh
dotnet user-secrets set Discord:Token "your-token"
```

For deployment, use an environment variable instead:

```sh
# bash
export Discord__Token="your-token"
# PowerShell
$env:Discord__Token = "your-token"
```

The bot stops with an error at startup if no token is configured.

### 5. Run

```sh
dotnet run
```

Global slash commands can take a while to show up. While developing, set `Discord:TestGuildId` to your test server's ID (with `DebugMode` on) so slash commands are registered to that server instantly.

## Configuration

All settings live under the `Discord` section of `appsettings.json` and can be overridden with user secrets or environment variables (`Discord__<Key>`).

| key           | description                                                                                         |
|---------------|-----------------------------------------------------------------------------------------------------|
| `Token`       | Bot token                                                                                           |
| `Prefix`      | Prefix for text commands                                                                            |
| `Intents`     | Gateway intents, e.g. `AllUnprivileged, MessageContent` (defaults to `AllUnprivileged`)             |
| `DebugMode`   | Shows detailed error messages in replies, and enables test guild registration                       |
| `TestGuildId` | Guild to register slash commands to while `DebugMode` is on. `0` registers them globally            |
| `LogSeverity` | Minimum Discord.Net log severity (`Critical`, `Error`, `Warning`, `Info`, `Verbose`, `Debug`)       |

## Features

| feature                | availability | description                                                                                                     |
|------------------------|------------|-----------------------------------------------------------------------------------------------------------------|
| Command                | ✅          | Classic Text Commands                                                                                          |
| Slash Command          | ✅          | Command with interactions                                                                                      |
| Paginator              | ✅          | Paginator for interaction and text command                                                                     |
| Automated Help         | ✅          | `help` / `/help` built from your modules, with autocomplete                                                    |
| Hot Reload             | ⚠️          | Reload command modules automatically when Hot Reload triggered<br/>(Doesn't work for removing existing module) |

### Commands

| command           | availability | description                                                             |
|-------------------|--------------|-------------------------------------------------------------------------|
| sudo run          | ✅          | Runs C# code                                                            |
| sudo status       | ✅          | Checks the bots information                                             |
| sudo su           | ✅          | Simulates a command as if the targeted user is using it.                |
| sudo reload       | ⚠️         | Reloads command modules<br/>(Doesn't work for removing existing module) |
| sudo sh           | ✅          | Executes command line on terminal (times out after 30 seconds)          |
| help              | ✅          | Automated Help command                                                  |

`sudo` commands are only available to the bot's owner.

## Adding Commands

Modules are discovered automatically; just add a class.

### Text command

```cs
// Modules/Commands/Greeting.cs
public class Greeting : ModuleBase<SocketCommandContext>
{
    [Command("hello")]
    [Summary("Says hello")]
    public async Task Hello()
    {
        await Context.ReplyAsync($"Hello, {Context.User.Username}!");
    }
}
```

### Slash command

```cs
// Modules/Interactions/Greeting.cs
public class Greeting : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("hello", "Says hello")]
    public async Task Hello()
    {
        await Context.RespondAsync($"Hello, {Context.User.Username}!");
    }
}
```

### Help attributes

- Commands appear in help only if they have a `[Summary]` (text) or description (slash).
- `[Order(n)]` on a module sets its page order in help.
- `[HideInHelp]` on a module or command hides it from help.

## Adding a Paginator

Create a class deriving from `Paginator` and mark a method with `[Paginator("id")]`. The method returns the page embed and whether it's the last page; `Index` holds the current page.

```cs
// Modules/Paginators/Numbers.cs
public class Numbers : Paginator
{
    [Paginator("numbers")]
    public (Embed, bool) Page()
    {
        var embed = new EmbedBuilder().WithDescription($"Page {Index + 1}").Build();

        return (embed, Index >= 4);
    }
}
```

Then start it from any command by injecting `PaginatorHandler`:

```cs
public class NumbersCommand(PaginatorHandler paginator) : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("numbers", "Shows numbers")]
    public async Task Numbers()
    {
        await paginator.InitPaginator(Context, "numbers");
    }
}
```

- Paginator IDs must not contain `:`.
- The page method takes no parameters and returns `(Embed, bool)` or `Task<(Embed, bool)>`.
- Paginators are created per page in their own DI scope, so they can inject scoped services through the constructor.
- Page buttons only respond to the user who ran the command, and keep working after the bot restarts.

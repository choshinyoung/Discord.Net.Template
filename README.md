# Discord.Net.Template

[Discord.Net](https://github.com/discord-net/Discord.Net) Template project for .NET 10

To use this template, follow these steps:

1. Clone the repository.
2. Run `dotnet new install .` to install the template.
3. Run `dotnet new dnettemplate` to create a new Discord.Net project.

## Features

| feature                | availability | description                                                                                                     |
|------------------------|------------|-----------------------------------------------------------------------------------------------------------------|
| Command                | ✅          | Classic Text Commands                                                                                          |
| Slash Command          | ✅          | Command with interactions                                                                                      |
| Paginator              | ✅          | Paginator for interaction and text command                                                                     |
| Hot Reload             | ⚠️          | Reload command modules automatically when Hot Reload triggered<br/>(Doesn't work for removing existing module) |

### Commands

| command           | availability | description                                                             |
|-------------------|--------------|-------------------------------------------------------------------------|
| sudo run          | ✅          | Runs C# code                                                            |
| sudo status       | ✅          | Checks the bots information                                             |
| sudo su           | ✅          | Simulates a command as if the targeted user is using it.                |
| sudo reload       | ⚠️         | Reloads command modules<br/>(Doesn't work for removing existing module) |
| sudo sh           | ✅          | Executes command line on terminal                                       |
| help              | ✅          | Automated Help command                                                  |

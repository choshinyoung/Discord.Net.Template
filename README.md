# Discord.Net Template Project

To use this template, follow these steps:

1. Clone the repository.
2. For CLI or Visual Studio: navigate to the cloned repository location and run `dotnet new install .\ `.
3. For Rider: install the template on the `More Templates` tab.

## Supports

| feature                | isComplete | description                                                                                                    |
|------------------------|------------|----------------------------------------------------------------------------------------------------------------|
| Command                | ✅          | Classic Text Commands                                                                                          |
| Slash Command          | ✅          | Command with interactions                                                                                      |
| Hot Reload             | ⚠️         | Reload command modules automatically when Hot Reload triggered<br/>(Doesn't work for removing existing module) |

### Commands

| command           | isComplete | description                                                             |
|-------------------|------------|-------------------------------------------------------------------------|
| sudo run          | ✅          | Runs C# code                                                            |
| sudo status       | ✅          | Checks the bots information                                             |
| sudo su           | ✅          | Simulates a command as if the targeted user is using it.                |
| sudo reload       | ⚠️         | Reloads command modules<br/>(Doesn't work for removing existing module) |
| sudo restart      | ✅          | Restarts program                                                        |
| sudo sh           | ❌          | Executes terminal command line                                          |
| Temporary Command | ❌          | Builds temporary command on runtime                                     |
| Git               | ❌          | Git supports                                                            |
| help              | ✅          | Automated Help command                                                  |

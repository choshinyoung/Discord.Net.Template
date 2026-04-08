using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.Net.Template.Services;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

var intents = builder.Configuration.GetValue("Discord:Intents", GatewayIntents.AllUnprivileged);
var logLevel = builder.Configuration.GetValue("Discord:LogSeverity", LogSeverity.Warning);

builder.Services.AddSingleton(
    new DiscordSocketClient(new() { GatewayIntents = intents, LogLevel = logLevel })
);

builder.Services.AddSingleton(x => new InteractionService(
    x.GetRequiredService<DiscordSocketClient>(),
    new InteractionServiceConfig
    {
        DefaultRunMode = Discord.Interactions.RunMode.Async,
        LogLevel = logLevel,
    }
));

builder.Services.AddSingleton(x => new CommandService(
    new() { DefaultRunMode = Discord.Commands.RunMode.Async, LogLevel = logLevel }
));

builder.Services.AddSingleton<IModuleHandler, InteractionHandler>();
builder.Services.AddSingleton<IModuleHandler, CommandHandler>();

builder.Services.AddSingleton<PaginatorHandler>();
builder.Services.AddSingleton<IModuleHandler>(sp => sp.GetRequiredService<PaginatorHandler>());

builder.Services.AddHostedService<BotService>();

var app = builder.Build();
HotReloadHandler.ServiceProvider = app.Services;

await app.RunAsync();

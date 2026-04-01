using Discord;
using Discord.Interactions;
using Discord.Net.Template.Services;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

var intents =
    builder
        .Configuration.GetSection("Bot:Intents")
        .Get<GatewayIntents[]>()
        ?.Aggregate((a, b) => a | b)
    ?? GatewayIntents.AllUnprivileged;

var logLevel =
    builder.Configuration.GetSection("Bot:LogSeverity")?.Get<LogSeverity>() ?? LogSeverity.Warning;

builder.Services.AddSingleton(
    new DiscordSocketClient(new() { GatewayIntents = intents, LogLevel = logLevel })
);

builder.Services.AddSingleton(x => new InteractionService(
    x.GetRequiredService<DiscordSocketClient>(),
    new InteractionServiceConfig { DefaultRunMode = RunMode.Async, LogLevel = logLevel }
));

builder.Services.AddSingleton<IModuleHandler, InteractionHandler>();
builder.Services.AddHostedService<BotService>();

var app = builder.Build();
await app.RunAsync();

using Discord;
using Discord.Interactions;
using Discord.Net.Template.Services;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
        config
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build()
    )
    .ConfigureServices(
        (context, services) =>
        {
            var intents = (context.Configuration.GetSection("Bot:Intents").Get<string[]>() ?? [])
                .Select(Enum.Parse<GatewayIntents>)
                .Aggregate((a, b) => a | b);

            var logLevel = Enum.Parse<LogSeverity>(
                context.Configuration["Bot:LogSeverity"] ?? "Info"
            );

            services.AddSingleton(
                new DiscordSocketClient(new() { GatewayIntents = intents, LogLevel = logLevel })
            );

            services.AddSingleton(x => new InteractionService(
                x.GetRequiredService<DiscordSocketClient>(),
                new InteractionServiceConfig { DefaultRunMode = RunMode.Async, LogLevel = logLevel }
            ));

            services.AddSingleton<IModuleHandler, InteractionHandler>();
            services.AddHostedService<BotService>();
        }
    )
    .Build();

await host.RunAsync();

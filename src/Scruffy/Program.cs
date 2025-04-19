using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Scruffy.Data;
using Scruffy.Services;

var builder = Host.CreateApplicationBuilder(args);

// Setup data structure
builder.Services.AddDbContext<ScruffyDbContext>();

var socketConfig = new DiscordSocketConfig
{
    LogLevel = LogSeverity.Verbose,
    GatewayIntents =
GatewayIntents.Guilds |
           GatewayIntents.GuildMessages,
    UseInteractionSnowflakeDate = false,
};

var client = new DiscordSocketClient(socketConfig);
builder.Services.AddSingleton(client);
builder.Services.AddSingleton<CommandService>();
builder.Services.AddSingleton<BotCommandService>();
builder.Services.AddSingleton(new InteractionService(client));
builder.Services.AddSingleton<StartupService>();

builder.Services.AddHostedService<DiscordBotService>();

var host = builder.Build();
host.Run();

using Discord;
using Discord.WebSocket;

namespace Scruffy.Services;

public class DiscordBotService(
    StartupService startupService,
    DiscordSocketClient discordSocketClient,
    ILogger<DiscordBotService> logger,
    BotCommandService commandService)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await startupService.StartAsync();

        while (discordSocketClient.CurrentUser == null)
        {
            logger.LogInformation("Discord user connection pending ...");
            await Task.Delay(5000, cancellationToken)
                .ConfigureAwait(false);
        }

        while (discordSocketClient.ConnectionState != ConnectionState.Connected)
        {
            logger.LogInformation("Discord user connection pending ...");
            await Task.Delay(5000, cancellationToken)
                .ConfigureAwait(false);
        }

        logger.LogInformation($"Discord user connected: {discordSocketClient.CurrentUser.Username}");

        await commandService.Init();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Bot stopping");
        return Task.CompletedTask;
    }
}
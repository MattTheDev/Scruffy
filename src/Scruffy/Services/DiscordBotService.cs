using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Scruffy.Data;

namespace Scruffy.Services;

public class DiscordBotService(
    StartupService startupService,
    DiscordSocketClient discordSocketClient,
    ILogger<DiscordBotService> logger,
    BotCommandService commandService,
    IServiceScopeFactory serviceScopeFactory)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Ensure database is configured / created.
        var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ScruffyDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);

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

        logger.LogInformation("Discord user connected: {User}",
            discordSocketClient.CurrentUser.Username);

        await commandService.Init();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Bot stopping");
        return Task.CompletedTask;
    }
}

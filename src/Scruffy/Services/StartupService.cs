using Discord;
using Discord.WebSocket;

namespace Scruffy.Services;

public class StartupService(
    DiscordSocketClient discordSocketClient,

    ILogger<StartupService> logger)
{
    public async Task StartAsync()
    {
        logger.LogInformation("Starting connection to Discord ...");
        var discordToken = Environment.GetEnvironmentVariable("SCRUFFY_TOKEN");

        if (string.IsNullOrWhiteSpace(discordToken))
        {
            logger.LogCritical("Discord token is missing. Create your bot and add an environment variable called 'SCRUFFY_TOKEN' and store it there.");
            throw new ArgumentNullException(discordToken);
        }

        await discordSocketClient.LoginAsync(TokenType.Bot, discordToken)
            .ConfigureAwait(false);
        await discordSocketClient.StartAsync()
            .ConfigureAwait(false);

        logger.LogInformation("Connection to Discord Established ...");
    }
}
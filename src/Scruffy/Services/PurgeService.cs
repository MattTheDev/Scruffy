using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Scruffy.Data;

namespace Scruffy.Services;

/// <summary>
/// This service will pull messages on a configured channel, and if the PurgeInterval was breached,
/// it will delete the message.
/// </summary>
/// <param name="serviceScopeFactory"></param>
/// <param name="logger"></param>
/// <param name="discordSocketClient"></param>
public class PurgeService(IServiceScopeFactory serviceScopeFactory,
    ILogger<PurgeService> logger,
    DiscordSocketClient discordSocketClient) 
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var scope = serviceScopeFactory.CreateScope();
        
        while (!cancellationToken.IsCancellationRequested)
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ScruffyDbContext>();

            var channels = await dbContext
                .Channels
                .ToListAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            foreach (var channel in channels)
            {
                var guild = discordSocketClient.GetGuild(ulong.Parse(channel.GuildId));
                var guildChannel = (ISocketMessageChannel)guild.GetChannel(ulong.Parse(channel.ChannelId));
var allMessages = new List<IMessage>();

                await foreach (var messageBatch in guildChannel.GetMessagesAsync().WithCancellation(cancellationToken))
                {
                    allMessages.AddRange(messageBatch);
                }

                var messagesToRemove = allMessages.Where(x =>
                    x.CreatedAt.DateTime > DateTime.UtcNow.AddMinutes(channel.PurgeInterval))
                    .ToList();

                foreach (var message in messagesToRemove)
                {
                    await guildChannel.DeleteMessageAsync(message)
                        .ConfigureAwait(false);
                }

                if (messagesToRemove.Count == 0)
                {
                    continue;
                }

                channel.PurgeCount += messagesToRemove.Count;
                dbContext
                    .Channels
                    .Update(channel);
                await dbContext
                    .SaveChangesAsync(cancellationToken)
                    .ConfigureAwait(false);
            }

            await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken)
                .ConfigureAwait(false);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Service shutting down.");
        return Task.CompletedTask;
    }
}
using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using Scruffy.Data;

namespace Scruffy.Commands.Slash;

public class Channel(IServiceScopeFactory serviceScopeFactory) : InteractionModuleBase
{
    [SlashCommand("channel",
        "Configure a Scruffy channel",
        true,
        RunMode.Async)]
    public async Task ConfigureChannelAsync(IGuildChannel channel,
        int purgeInterval)
    {
        await DeferAsync(ephemeral: true);

        var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ScruffyDbContext>();

        var existingChannel = await dbContext
            .Channels
            .FirstOrDefaultAsync(x => x.ChannelId.Equals(channel.Id.ToString()))
            .ConfigureAwait(false);

        // Validate the input.
        // > 5 minutes and != 0 and the doesnt channel exist (0 is our removal criteria)
        // < 10080 = One Week
        if ((purgeInterval <= 5 && (purgeInterval == 0 && existingChannel == null)) ||
            purgeInterval > 10080)
        {
            await FollowupAsync("Sorry, the purge interval cannot be under 5 minutes or over 1 week.", 
                    ephemeral: true)
                .ConfigureAwait(false);

            return;
        }


        if (existingChannel == null)
        {
            // Create a new channel configuration.
            existingChannel = new Data.Entities.Channel
            {
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                ChannelId = channel.Id.ToString(),
                GuildId = Context.Guild.Id.ToString(),
                PurgeCount = 0,
                PurgeInterval = purgeInterval,
            };

            await dbContext
                .Channels
                .AddAsync(existingChannel)
                .ConfigureAwait(false);
            await dbContext
                .SaveChangesAsync()
                .ConfigureAwait(false);

            await FollowupAsync(
                    $"Your channel has been configured. Once a message hits {purgeInterval} minutes old, it will be purged from {channel.Name}.",
                    ephemeral: true)
                .ConfigureAwait(false);
        }
        else
        {
            // If a 0 purge interval has been configured, remove the configuration.
            if (purgeInterval == 0)
            {
                dbContext
                    .Channels
                    .Remove(existingChannel);
                await dbContext
                    .SaveChangesAsync()
                    .ConfigureAwait(false);

                await FollowupAsync($"Your configuration for {channel.Name} has been removed.",
                        ephemeral: true)
                    .ConfigureAwait(false);

                return;
            }

            // Update the purge interval.
            existingChannel.PurgeInterval = purgeInterval;
            existingChannel.ModifiedDate = DateTime.UtcNow;

            dbContext
                .Channels
                .Update(existingChannel);
            await dbContext
                .SaveChangesAsync()
                .ConfigureAwait(false);

            await FollowupAsync(
                    $"Your channel configuration has been updated. Once a message hits {purgeInterval} minutes old, it will be purged from {channel.Name}.",
                    ephemeral: true)
                .ConfigureAwait(false);
        }
    }
}
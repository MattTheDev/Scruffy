using Discord;
using Discord.Interactions;
using Microsoft.EntityFrameworkCore;
using Scruffy.Data;
using Scruffy.Data.Entities;

namespace Scruffy.Commands.Slash;

[Group("point", "Command group for Point system")]
public class Points(IServiceScopeFactory serviceScopeFactory) : InteractionModuleBase
{
    [SlashCommand("leaderboard",
        "View leaderboard of points granted",
        false,
        RunMode.Async)]
    public async Task LeaderboardAsync(bool global = true)
    {
        // Global Leaderboard Goes Here
    }

    [SlashCommand("grant",
        "Grant a point to a user",
        false,
        RunMode.Async)]
    public async Task GrantAsync(IGuildUser guildUser)
    {
        await DeferAsync();

        var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ScruffyDbContext>();

        var existingUser = await dbContext
            .Users
            .FirstOrDefaultAsync(x => x.Id.Equals(guildUser.Id.ToString()))
            .ConfigureAwait(false);

        if (existingUser == null)
        {
            existingUser = new User
            {
                Id = guildUser.Id.ToString(),
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                Points = 0
            };

            await dbContext
                .Users
                .AddAsync(existingUser)
                .ConfigureAwait(false);
            await dbContext
                .SaveChangesAsync()
                .ConfigureAwait(false);
        }

        existingUser.Points++;
        
        dbContext
            .Users
            .Update(existingUser);
        await dbContext
            .SaveChangesAsync()
            .ConfigureAwait(false);

        var pointLog = new PointLog
        {
            GrantorId = Context.User.Id.ToString(),
            GranteeId = guildUser.Id.ToString(),
            GrantedDate = DateTime.UtcNow
        };

        await dbContext
            .PointLogs
            .AddAsync(pointLog)
            .ConfigureAwait(false);
        await dbContext
            .SaveChangesAsync()
            .ConfigureAwait(false);

        await FollowupAsync($"{Context.User.Username} has granted a point to {guildUser.Username}.");
    }
}
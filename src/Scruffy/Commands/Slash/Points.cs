using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Scruffy.Data;
using Scruffy.Data.Entities;

namespace Scruffy.Commands.Slash;

[Group("point", "Command group for Point system")]
public class Points(IServiceScopeFactory serviceScopeFactory,
    DiscordSocketClient discordSocketClient) : InteractionModuleBase
{
    [SlashCommand("leaderboard",
        "View leaderboard of points granted",
        false,
        RunMode.Async)]
    public async Task LeaderboardAsync(bool global = true)
    {
        await DeferAsync();

        var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ScruffyDbContext>();

        var top10Points = await dbContext
            .Users
            .OrderByDescending(x => x.Points)
            .Take(10)
            .ToListAsync()
            .ConfigureAwait(false);

        var embedBuilder = new EmbedBuilder();
        var authorBuilder = new EmbedAuthorBuilder();
        var footerBuilder = new EmbedFooterBuilder();

        authorBuilder.IconUrl = discordSocketClient.CurrentUser.GetAvatarUrl();
        authorBuilder.Name = "Scruffy the Server Janitor";
        authorBuilder.Url = "https://mattthedev.codes";

        footerBuilder.IconUrl = discordSocketClient.CurrentUser.GetAvatarUrl();
        footerBuilder.Text = "Cleaning Servers since 4/20/2025";

        embedBuilder.Author = authorBuilder;
        embedBuilder.Footer = footerBuilder;
        embedBuilder.WithCurrentTimestamp();

        embedBuilder.ThumbnailUrl = "https://mattthedev.codes/wp-content/uploads/2020/11/mtdCODES.png";
        embedBuilder.Title = "Top 10 Global Point Leaders";
        embedBuilder.Description =
            "Points are granted between users for counterpoints, well thought out posts, etc. Below are the top 10 point leaders globally.";

        var userList = new List<(string, int)>();
        foreach (var user in top10Points)
        {
            var u = await discordSocketClient.GetUserAsync(ulong.Parse(user.Id));
            userList.Add((u.Username, user.Points));
        }

        embedBuilder.AddField(new EmbedFieldBuilder
        {
            IsInline = true,
            Name = "Leaders",
            Value = string.Join(' ', userList.Select(x => $"{x.Item1} - {x.Item2}\r\n"))
        });

        await FollowupAsync(embed: embedBuilder.Build());
    }

    [SlashCommand("grant",
        "Grant a point to a user",
        false,
        RunMode.Async)]
    public async Task GrantAsync(IGuildUser guildUser,
        string messageId)
    {
        await DeferAsync();

        if (!ulong.TryParse(messageId, out var validMessageId))
        {
            await FollowupAsync("Invalid message ID was provided. Try again.");
            return;
        }

        var validMessage = await Context
            .Channel
            .GetMessageAsync(validMessageId)
            .ConfigureAwait(false);

        if (validMessage == null)
        {
            await FollowupAsync("Invalid message ID was provided. Try again.");
            return;
        }

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
            GrantedDate = DateTime.UtcNow,
            GuildId = Context.Guild.Id.ToString(),
        };

        await dbContext
            .PointLogs
            .AddAsync(pointLog)
            .ConfigureAwait(false);
        await dbContext
            .SaveChangesAsync()
            .ConfigureAwait(false);

        await FollowupAsync($"{Context.User.Mention} has granted a point to {guildUser.Mention} for: \r\n\r\n" +
                            $"https://discord.com/channels/{Context.Guild.Id}/{Context.Channel.Id}/{messageId}");
    }
}
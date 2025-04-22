using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Scruffy.Data;

namespace Scruffy.Commands.Slash;

public class Utility(IServiceScopeFactory serviceScopeFactory,
    ILogger<Utility> logger,
    DiscordSocketClient discordSocketClient) : InteractionModuleBase
{
    [SlashCommand("ping",
        "You want da pong?!",
        true,
        RunMode.Async)]
    public async Task PingAsync()
    {
        await RespondAsync("Pong!")
            .ConfigureAwait(false);
    }

    [SlashCommand("info",
        "Information and Bot stats",
        true,
        RunMode.Async)]
    public async Task InfoAsync()
    {
        await DeferAsync();

        var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ScruffyDbContext>();
        var channels = await dbContext
            .Channels
            .ToListAsync()
            .ConfigureAwait(false);
        var pointLogCount = await dbContext
            .PointLogs
            .CountAsync()
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
        embedBuilder.Title = "What is Scruffy?";
        embedBuilder.Description =
            "Scruffy was created by MTD to moderate and clean the #thunderdome channel in the CouchBot Community discord Server. " +
            "Scruffy grants access to and purges old messages in a channel made for debate, posting things that make us grumpy, or rants. " +
            "These messages are only meant to be timeboxed .. hence Scruffy!";

        var serverCount = channels.Select(x => x.GuildId).Distinct().Count();
        var channelCount = channels.Select(x => x.ChannelId).Distinct().Count();
        var purgeCount = channels.Sum(x => x.PurgeCount);
        var averagePurgeTime = channels.Average(x => x.PurgeInterval);

        embedBuilder.AddField(new EmbedFieldBuilder
        {
IsInline = true,
Name = "Stats",
Value = $"Servers: {serverCount}\r\n" +
        $"Channels: {channelCount}\r\n" +
        $"Purged Messages: {purgeCount}\r\n" +
        $"Average Purge Interval: {averagePurgeTime}\r\n" +
        $"Points Granted: {pointLogCount}"
        });

        await FollowupAsync(embed: embedBuilder.Build());
    }

    [SlashCommand("echo",
        "Echo in an embed!",
        true,
        RunMode.Async)]
    public async Task EchoAsync(string message)
    {
        await DeferAsync();

        var embedBuilder = new EmbedBuilder
        {
            Description = message
        };

        await FollowupAsync(embed: embedBuilder.Build());
    }
}
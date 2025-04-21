using Discord;
using Discord.Interactions;

namespace Scruffy.Commands.Slash;

[Group("point", "Command group for Point system")]
public class Points : InteractionModuleBase
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
        // Grant logic goes here
    }
}
using Discord.Interactions;

namespace Scruffy.Commands.User;

public class Points : InteractionModuleBase
{
    [UserCommand("grant")]
    public async Task GrantAsync()
    {
        await DeferAsync();

        await FollowupAsync("Context Menu Command Clicked");
    }
}
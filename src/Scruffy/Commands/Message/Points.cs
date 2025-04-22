using Discord;
using Discord.Interactions;

namespace Scruffy.Commands.Message;

public class Points : InteractionModuleBase<SocketInteractionContext>
{
    [CommandContextType(InteractionContextType.Guild)]
    [IntegrationType(Discord.ApplicationIntegrationType.UserInstall, ApplicationIntegrationType.GuildInstall)]
[MessageCommand("grant")]
    public async Task GrantAsync()
    {
        //await DeferAsync();

        await RespondAsync("Context Menu Command Clicked");
    }
}
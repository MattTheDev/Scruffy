using Discord;
using Discord.WebSocket;

namespace Scruffy.Services;

public class MessageInteractionService(DiscordSocketClient discordSocketClient)
{
    public void Init()
    {
        discordSocketClient.ButtonExecuted += ButtonExecuted;
    }

    private async Task ButtonExecuted(SocketMessageComponent arg)
    {
        var customId = arg.Data.CustomId;
        var splitCustomId = customId.Split(":");

        switch (splitCustomId[0])
        {
            case "rr":
                await ToggleRole((IGuildUser)arg.User, splitCustomId[1]);
                break;
            default:
                // Nothing. There is nothing else.
                break;
        }
    }

    private async Task ToggleRole(IGuildUser user, string roleId)
    {
        var hasRole = user.RoleIds.Any(x => x == ulong.Parse(roleId));

        try
        {
            if (hasRole)
            {
                await user.RemoveRolesAsync([ulong.Parse(roleId)]);
            }
            else
            {
                await user.AddRoleAsync(ulong.Parse(roleId));
            }
        }
        catch (Exception)
        {
            // It's fine. This is fine. TODO MS - Logging
        }
    }
}
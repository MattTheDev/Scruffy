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
        await arg.DeferAsync(ephemeral: true);

        var customId = arg.Data.CustomId;
        var splitCustomId = customId.Split(":");
        
        var hasRole = false;
        switch (splitCustomId[0])
        {
            case "rr":
                hasRole = await ToggleRole((IGuildUser)arg.User, splitCustomId[1]);
                break;
            default:
                // Nothing. There is nothing else.
                break;
        }

        var status = hasRole ? "added" : "removed";
        await arg.FollowupAsync($"Role has been {status}.", ephemeral: true);
    }

    private async Task<bool> ToggleRole(IGuildUser user, string roleId)
    {
        var hasRole = user.RoleIds.Any(x => x == ulong.Parse(roleId));

        try
        {
            if (hasRole)
            {
                await user.RemoveRolesAsync([ulong.Parse(roleId)]);
                return false;
            }

            await user.AddRoleAsync(ulong.Parse(roleId));
            return true;
        }
        catch (Exception)
        {
            // It's fine. This is fine. TODO MS - Logging
        }

        return false;
    }
}
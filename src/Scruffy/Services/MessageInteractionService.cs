using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Scruffy.Data;

namespace Scruffy.Services;

public class MessageInteractionService(DiscordSocketClient discordSocketClient,
    IServiceScopeFactory serviceScopeFactory)
{
    public void Init()
    {
        discordSocketClient.ReactionAdded += ReactionAdded;
    }

    private async Task ReactionAdded(Cacheable<IUserMessage, ulong> arg1, 
        Cacheable<IMessageChannel, ulong> arg2, 
        SocketReaction arg3)
    {
        var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ScruffyDbContext>();
        var guild = ((IGuildChannel)arg3.Channel).Guild;
        var reactionRole = await dbContext
            .Roles
            .FirstOrDefaultAsync(x => x.GuildId.Equals(guild.Id.ToString()) &&
                        x.MessageId.Equals(arg3.MessageId.ToString()) &&
                        x.Emote.Equals(arg3.Emote))
            .ConfigureAwait(false);

        if (reactionRole == null)
        {
            return;
        }

        var role = guild.Roles.FirstOrDefault(x => x.Id == ulong.Parse(reactionRole.RoleId));

        if (role == null)
        {
            return;
        }

        var guildUser = (IGuildUser)arg3.User.Value;
        var hasRole = guildUser.RoleIds.Any(x => x == ulong.Parse(reactionRole.RoleId));

        try
        {
            if (hasRole)
            {
                await guildUser.RemoveRolesAsync([ulong.Parse(reactionRole.RoleId)]);
            }
            else
            {
                await guildUser.AddRoleAsync(ulong.Parse(reactionRole.RoleId));
            }
        }
        catch (Exception)
        {
            // It's fine. This is fine. TODO MS - Logging
        }
    }
}
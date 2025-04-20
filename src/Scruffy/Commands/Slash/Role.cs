using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Scruffy.Data;

namespace Scruffy.Commands.Slash;


public class Role(DiscordSocketClient discordSocketClient,
    IServiceScopeFactory serviceScopeFactory) : InteractionModuleBase
{
    [SlashCommand("role",
        "Command to configure reaction, role, and message for access.",
        true,
        RunMode.Async)]
    public async Task SetupAsync(string messageId,
        string emote,
        IRole role)
    {
        await DeferAsync(true);

        var validMessage = await Context.Channel.GetMessageAsync(ulong.Parse(messageId)).ConfigureAwait(false);

        if (validMessage == null)
        {
            await FollowupAsync("Please provide a valid message ID.");
            return;
        }

        var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ScruffyDbContext>();
        var existingRole = await dbContext.Roles.FirstOrDefaultAsync(x =>
                x.Emote.Equals(emote) &&
                x.RoleId.Equals(role.Id.ToString()) &&
                x.MessageId.Equals(messageId))
            .ConfigureAwait(false);

        if (existingRole != null)
        {
            await FollowupAsync("Role configurations must be unique.");
            return;
        }

        var newRole = new Data.Entities.Role
        {
            MessageId = messageId,
            Emote = emote,
            RoleId = role.Id.ToString(),
        };

        var emoteParts = emote.TrimStart('<').TrimEnd('>').Split(':', StringSplitOptions.RemoveEmptyEntries);

        IEmote guildEmote;

        if (emoteParts.Length > 1)
        {
            guildEmote = Context
                .Guild
                .Emotes
                .FirstOrDefault(x => x.Id == ulong.Parse(emoteParts[0]));
        }
        else
        {
            guildEmote = new Emoji(emoteParts[0]);
        }

        await validMessage
            .AddReactionAsync(guildEmote)
            .ConfigureAwait(false);

        await dbContext
            .Roles
            .AddAsync(newRole)
            .ConfigureAwait(false);
        await dbContext
            .SaveChangesAsync()
            .ConfigureAwait(false);

        await FollowupAsync("Reaction role has been successfully added.");
    }
}
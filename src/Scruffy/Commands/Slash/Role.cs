using Discord;
using Discord.Interactions;

namespace Scruffy.Commands.Slash;


public class Role : InteractionModuleBase
{
    [SlashCommand("role",
        "Command to configure reaction, role, and message for access.",
        true,
        RunMode.Async)]
    public async Task SetupAsync(string messageId,
        string emote,
        string label,
        IRole role)
    {
        await DeferAsync(true);

        var validMessage = await Context.Channel.GetMessageAsync(ulong.Parse(messageId)).ConfigureAwait(false);

        if (validMessage == null)
        {
            await FollowupAsync("Please provide a valid message ID.");
            return;
        }

        var componentBuilder = new ComponentBuilder()
            .WithButton(label,
                $"rr:{role.Id}",
                emote: new Emoji(emote));

        await Context.Channel.ModifyMessageAsync(validMessage.Id, (properties) =>
        {
            properties.Components = new Optional<MessageComponent>(componentBuilder.Build());
        });

        await FollowupAsync("Role button has been successfully added.");
    }
}
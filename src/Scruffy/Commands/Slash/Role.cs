﻿using Discord;
using Discord.Interactions;

namespace Scruffy.Commands.Slash;


public class Role : InteractionModuleBase
{
    [SlashCommand("role",
        "Command to configure reaction, role, and message for access.",
        true,
        RunMode.Async)]
    public async Task SetupAsync(string body, 
        string emote,
        string label,
        IRole role)
    {
        await DeferAsync();

        var embedBuilder = new EmbedBuilder
        {
            Description = body
        };

        var componentBuilder = new ComponentBuilder()
            .WithButton(label,
                $"rr:{role.Id}",
                emote: new Emoji(emote)); ;

        await FollowupAsync(embed: embedBuilder.Build(),
            components: componentBuilder.Build());
    }
}
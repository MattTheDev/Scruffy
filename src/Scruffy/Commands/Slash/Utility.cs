using Discord.Interactions;

namespace Scruffy.Commands.Slash;

public class Utility : InteractionModuleBase
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
}
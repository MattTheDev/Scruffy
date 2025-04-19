using Discord.Commands;
using Discord.Interactions;
using RunMode = Discord.Commands.RunMode;

namespace Scruffy.Commands.Slash;

public class Utility : InteractionModuleBase
{
    [Command("ping",
        true,
        "You want da pong?!",
        RunMode = RunMode.Async)]
    public async Task PingAsync()
    {
        await RespondAsync("Pong!")
            .ConfigureAwait(false);
    }
}
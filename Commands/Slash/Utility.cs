using System.Runtime.InteropServices;
using Discord.Commands;

namespace Scruffy.Commands.Slash;

public class Utility : ModuleBase
{
    [Command("ping",
        true,
        "You want da pong?!",
        RunMode = RunMode.Async)]
    public async Task PingAsync()
    {
        await ReplyAsync("Pong!")
            .ConfigureAwait(false);
    }
}
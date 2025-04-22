using System.Reflection;
using Discord.Interactions;
using Discord.WebSocket;

namespace Scruffy.Services;

public class BotCommandService(DiscordSocketClient discordSocketClient,
    IServiceProvider serviceProvider,
    InteractionService interactionService)
{
    public async Task Init()
    {
        await interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), serviceProvider)
            .ConfigureAwait(false);

        await interactionService.RegisterCommandsGloballyAsync()
            .ConfigureAwait(false);

        discordSocketClient.SlashCommandExecuted += async interaction =>
        {
            var ctx = new SocketInteractionContext<SocketSlashCommand>(discordSocketClient, interaction);
            await interactionService.ExecuteCommandAsync(ctx, serviceProvider)
                .ConfigureAwait(false);
        };  
    }
}
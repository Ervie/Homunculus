using Discord.Commands;
using Discord.WebSocket;
using MarekMotykaBot.Resources;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MarekMotykaBot.Services
{
    public class CommandHandlingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly MessageScannerService _scanner;
        private readonly IServiceProvider _provider;

        // DiscordSocketClient, CommandService, and IServiceProvider are injected automatically from the IServiceProvider
        public CommandHandlingService(DiscordSocketClient discord, CommandService commands, MessageScannerService scanner, IServiceProvider provider)
        {
            _discord = discord;
            _commands = commands;
            _scanner = scanner;
            _provider = provider;

            _discord.MessageReceived += OnMessageReceivedAsync;
            _discord.MessageReceived += scanner.ScanMessage;
        }

        public CommandService Commands => _commands;

        private async Task OnMessageReceivedAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;
            if (msg.Author == _discord.CurrentUser) return;

            var context = new SocketCommandContext(_discord, msg);

            int argPos = 0;
            if (msg.HasStringPrefix(".", ref argPos) || msg.HasMentionPrefix(_discord.CurrentUser, ref argPos))
            {
                if (!DeclineCommand(context, msg.Content).Result)
                {
                    var result = await Commands.ExecuteAsync(context, argPos, _provider);

                    if (!result.IsSuccess)
                        await context.Channel.SendMessageAsync(result.ToString());
                }
            }
        }

        private async Task<bool> DeclineCommand(SocketCommandContext context, string messageContent)
        {
            string commandName = messageContent.Split(' ').FirstOrDefault();
            bool commandDeclined = false;

            if (!string.IsNullOrWhiteSpace(commandName))
            {
                string meanResponse = string.Format(StringConsts.DeclineCommand, commandName);

                Random rng = new Random();

                int randomInt = rng.Next(1, 10);

                // bad luck, you suck
                if (randomInt == 1)
                {
                    commandDeclined = true;
                    await context.Channel.SendMessageAsync(meanResponse);
                }
            }
            return commandDeclined;
        }
    }
}
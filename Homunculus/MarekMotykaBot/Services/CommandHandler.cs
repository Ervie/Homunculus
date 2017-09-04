using Discord.Commands;
using Discord.WebSocket;
using MarekMotykaBot.Resources;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MarekMotykaBot
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IServiceProvider _provider;

        public CommandHandler(IServiceProvider provider)
        {
            _provider = provider;
            _discord = _provider.GetService<DiscordSocketClient>();

            MessageScanner.Client = _discord;
            _commands = _provider.GetService<CommandService>();
        }

        public async Task InstallCommands()
        {
            _discord.MessageReceived += MessageScanner.ScanMessage;
            _discord.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            var message = s as SocketUserMessage;

            if (message == null)
                return;

            var context = new SocketCommandContext(_discord, message);

            int argPos = 0;

            if (message.HasStringPrefix("!", ref argPos) ||
                message.HasMentionPrefix(_discord.CurrentUser, ref argPos))
            {
                if (!DeclineCommand(context, message.Content).Result)
                {
                    var result = await _commands.ExecuteAsync(context, argPos);

                    if (result.IsSuccess && result.Error != CommandError.UnknownCommand)
                    {
                        await context.Channel.SendMessageAsync(result.ErrorReason);
                    }
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